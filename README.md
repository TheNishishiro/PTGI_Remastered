# Notice

Current branch is under heavy refactorization, some features are completely removed until migration from Alea to ILGPU is completed

Currently even without previous optimizations performance is greatly improved, up to real time rendering on GTX 1050m

# PTGI_Remastered

 Path traced global illumination is used to illuminate the scene by sending multiple rays from pixels and bouncing them around until it hits the light source or it reaches bounce limit. The light is then calculated based on information collected by the bouncing ray.
 
 Following project tries to replicate this behaviour in order to light up a 2D environment 

## Example screenshots
### Sample renders
#### ~30 fps scene render
<p align="center">
 <img src="https://github.com/TheNishishiro/PTGI_Remastered/blob/PTGI_ILGPU_Dev/Images/30fps.png?raw=true">
</p>

#### ~30 fps scene render (median denoiser, 9x9 kernel)
You can observe some light leaking through objects due to large kernel
<p align="center">
 <img src="https://github.com/TheNishishiro/PTGI_Remastered/blob/PTGI_ILGPU_Dev/Images/30fps_denoised.png?raw=true">
</p>

#### 10000 samples (26 seconds render)
<p align="center">
 <img src="https://github.com/TheNishishiro/PTGI_Remastered/blob/PTGI_ILGPU_Dev/Images/10000s@26s.png?raw=true">
</p>

#### Untitled
<p align="center">
 <img src="https://github.com/TheNishishiro/PTGI_Remastered/blob/PTGI_ILGPU_Dev/Images/image.png?raw=true">
 <img src="https://github.com/TheNishishiro/PTGI_Remastered/blob/PTGI_ILGPU_Dev/Images/image2.png?raw=true">
 <img src="https://github.com/TheNishishiro/PTGI_Remastered/blob/PTGI_ILGPU_Dev/Images/image3.png?raw=true">
 <img src="https://github.com/TheNishishiro/PTGI_Remastered/blob/PTGI_ILGPU_Dev/Images/image4.png?raw=true">
</p>

### User Interface (outdated)

<p align="center">
 <img src="https://github.com/TheNishishiro/PTGI_Remastered/blob/main/Images/ui.png?raw=true">
 <img src="https://github.com/TheNishishiro/PTGI_Remastered/blob/main/Images/ui2.png?raw=true">
</p>

## Project 

Solution contains two projects, 

- PTGI_Remastered, which is a library containing the renderer and logic itself, together with all structures to run with it.
- PTGI_UI, is just an interface to communicate with a library.

## Requirements 

It is recommended to run this program on a high end GPU but it should work just fine with low end GPU or even a CPU.

Current iteration is being tested on RTX 3080 and GTX 1050m

## Features

 Pretty much everything you'll expect from PT such as 
 
 - soft shadows
 - colored lights
 - color bleeding
 - a bunch of **noise**
 
## Improvements from old version

- Utilizes [ILGPU](https://github.com/m4rs-mt/ILGPU/wiki) for computation
- Improved code readability
- Structures instead of raw basic data types
- Renderer and UI seperated
