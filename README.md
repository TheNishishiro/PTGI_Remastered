# PTGI_Remastered

 Path traced global illumination is used to illuminate the scene by sending multiple rays from pixels and bouncing them around until it hits the light source or it reaches bounce limit. The light is then calculated based on informations collected by the bouncing ray.
 
 Following project tries to replicate this behaviour in order to light up a 2D environment 

## Example screenshots
### Renders (500 sppx)

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

Since GPU rendering requires CUDA to work you'll need CUDA capable GPU to utilize it.

For PTGI_Remastered you will need
```
 Alea 3.0.4
 FSharp.Core -Version 4.7.1
```
 
For PTGI_UI you will need 
```
 Alea 3.0.4
 Cyotek.Windows.Forms.ColorPicker -Version 1.7.2
 Control.Draggable -Version 1.0.5049.269
 MaterialSkin.2 -Version 2.1.3
```

## Features

 Pretty much everything you'll expect from PT such as 
 
 - soft shadows
 - colored lights
 - color bleeding
 - a bunch of **noise**
 
## Improvements from old version

 - Improved code readability
 - Structures instead of raw basic data types
 - Renderer and UI seperated
