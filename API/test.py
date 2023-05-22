import torch
from diffusers import StableDiffusionPipeline

from PIL import Image
import numpy as np
import torch
from torch.autograd import Variable
from torchvision import transforms
import torch.nn.functional as F
import warnings
warnings.filterwarnings("ignore")

import sys
sys.path.append('./dis_models')

from data_loader_cache import normalize, im_reader, im_preprocess 
from models import *

device = 'cuda'

# Stable Diffusion

pipe = StableDiffusionPipeline.from_pretrained("./diffusion_models/stable-diffusion-v1-4", torch_dtype=torch.float16, requires_safety_checker=False)
pipe.safety_checker = lambda images, **kwargs: (images, [False] * len(images))
pipe = pipe.to("cuda")

# DIS

class GOSNormalize(object):
	'''
	Normalize the Image using torch.transforms
	'''
	def __init__(self, mean=[0.485,0.456,0.406], std=[0.229,0.224,0.225]):
		self.mean = mean
		self.std = std

	def __call__(self,image):
		image = normalize(image,self.mean,self.std)
		return image


transform =  transforms.Compose([GOSNormalize([0.5,0.5,0.5],[1.0,1.0,1.0])])

def load_image(im_path, hypar):
	im = im_reader(im_path)
	im, im_shp = im_preprocess(im, hypar["cache_size"])
	im = torch.divide(im,255.0)
	shape = torch.from_numpy(np.array(im_shp))
	return transform(im).unsqueeze(0), shape.unsqueeze(0) # make a batch of image, shape


def build_model(hypar,device):
	net = hypar["model"]#GOSNETINC(3,1)

	# convert to half precision
	if(hypar["model_digit"]=="half"):
		net.half()
		for layer in net.modules():
			if isinstance(layer, nn.BatchNorm2d):
				layer.float()

	net.to(device)

	if(hypar["restore_model"]!=""):
		net.load_state_dict(torch.load(hypar["model_path"]+"/"+hypar["restore_model"], map_location=device))
		net.to(device)
	net.eval()  
	return net

	
def predict(net,  inputs_val, shapes_val, hypar, device):
	'''
	Given an Image, predict the mask
	'''
	net.eval()

	if(hypar["model_digit"]=="full"):
		inputs_val = inputs_val.type(torch.FloatTensor)
	else:
		inputs_val = inputs_val.type(torch.HalfTensor)

  
	inputs_val_v = Variable(inputs_val, requires_grad=False).to(device) # wrap inputs in Variable
   
	ds_val = net(inputs_val_v)[0] # list of 6 results

	pred_val = ds_val[0][0,:,:,:] # B x 1 x H x W    # we want the first one which is the most accurate prediction

	## recover the prediction spatial size to the orignal image size
	pred_val = torch.squeeze(F.upsample(torch.unsqueeze(pred_val,0),(shapes_val[0][0],shapes_val[0][1]),mode='bilinear'))

	ma = torch.max(pred_val)
	mi = torch.min(pred_val)
	pred_val = (pred_val-mi)/(ma-mi) # max = 1

	if device == 'cuda': torch.cuda.empty_cache()
	return (pred_val.detach().cpu().numpy()*255).astype(np.uint8) # it is the mask we need
	
# Set Parameters
hypar = {
	"model_path": "./dis_models/saved_models", ## load trained weights from this path
	"restore_model": "isnet.pth", ## name of the to-be-loaded weights
	"interm_sup": False, ## indicate if activate intermediate feature supervision
	"model_digit": "full", ## indicates "half" or "full" accuracy of float number
	"seed": 0,
	"cache_size": [512, 512], ## cached input spatial resolution, can be configured into different size
	"input_size": [1024, 1024], ## mdoel input spatial size, usually use the same value hypar["cache_size"], which means we don't further resize the images
	"crop_size": [1024, 1024], ## random crop size from the input, it is usually set as smaller than hypar["cache_size"], e.g., [920,920] for data augmentation
	"model": ISNetDIS()
} # paramters for inferencing

# Build Model
net = build_model(hypar, device)


def inference(image: Image):
	image_path = image
	
	image_tensor, orig_size = load_image(image_path, hypar) 
	mask = predict(net, image_tensor, orig_size, hypar, device)
	
	pil_mask = Image.fromarray(mask).convert("L")
	im_rgb = Image.open(image).convert("RGB")
	
	im_rgba = im_rgb.copy()
	im_rgba.putalpha(pil_mask)

	return [im_rgba, pil_mask]


image = pipe("Naked woman").images[0]
image.save(f"../Assets/Images/1.png")
image, _ = inference(f"../Assets/Images/1.png")
pixels = image.load()
emptyPixel = 0
for i in range(image.size[0]):    # for every col:
	for j in range(image.size[1]):    # For every row
		if (pixels[i, j][3] == 0):
			emptyPixel += 1
print(emptyPixel)
image.save(f"../Assets/Images/1_nobackground.png")
