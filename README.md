# DiffusedCollage

Final project repository for 4.453 Creative Machine Learning for Design (Spring 2023)

This repository serves directly as a Unity project. It can be opened via Unity Hub with Unity 2021.3.25f1 installed.

The *API* directory consists of [Stable Diffusion](https://github.com/huggingface/diffusers/tree/main) and [DIS (IS-Net) model](https://github.com/xuebinqin/DIS) hosted by Flask framework. Below is the instruction of executing it:

1. Download [Stable Diffusion Checkpoint](https://huggingface.co/CompVis/stable-diffusion-v1-4) and place the repository inside *API/diffusion_models/*.

2. Download [Pretrained IS-Net model](https://drive.google.com/file/d/1nV57qKuy--d5u1yvkng9aXW1KS4sOpOi/view?usp=sharing) amd place the file inside *API/dis_models/saved_models/*

3. Activate Conda environment and run Flask api:

```
cd API
conda env create -f environment.yml
conda activate diffusedcollage
python api.py //or python3
```

4. Currently, Unity project is hardcoded to make request at http://127.0.0.1:5000. If Flask appears to run on different address, please modifiy either Flask's setting at *Line 167, api.py* or Unity's requesting address at *Line 52, Assets/Scripts/ImageSpawner.cs*.

5. Run the Unity project and have fun!

6. The generated images can be found in *Assets/Images*. If an image has no corresponding *nobackground* image, it indicates the IS-Net wiped out almost everything as background.