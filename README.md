# Notice

Current branch is under heavy refactorization, some features are completely removed until migration from Alea to ILGPU is completed

In order to be up to date with Alea implementations:
- [x] Skip obscured pixels
- [ ] Grid traversal
- [ ] Transparent/Semi-transparent materials

Planned so far:
- [x] Allocation caching
- [ ] Grid traversal optimizations

Currently even without previous optimizations performance is greatly improved, up to real time rendering on GTX 1050m

# PTGI_Remastered

 Path traced global illumination is used to illuminate the scene by sending multiple rays from pixels and bouncing them around until it hits the light source or it reaches bounce limit. The light is then calculated based on information collected by the bouncing ray.
 
 Following project tries to replicate this behaviour in order to light up a 2D environment 

## Example screenshots
### Renders (500 sppx) (Outdated)

<p align="center">
 <img src="https://github.com/TheNishishiro/PTGI_Remastered/blob/main/Images/render1.png?raw=true">
 <img src="https://github.com/TheNishishiro/PTGI_Remastered/blob/main/Images/color%20bleeding.png?raw=true">
 <img src="https://github.com/TheNishishiro/PTGI_Remastered/blob/main/Images/render2.png?raw=true">
</p>

### User Interface

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
