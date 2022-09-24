# Notice

Current implementation is completely moved to ILGPU with only a couple things still missing (material properties for example have not been reimplemented yet as I focus on performance right now), readme and screenshots probably needs redoing at this point

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

#### ~30 fps scene render (mean denoiser, 18x18 kernel)
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

### Numbers


BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.22000.978/21H2)
11th Gen Intel Core i7-11700KF 3.60GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK=6.0.100-rc.2.21505.57
[Host]     : .NET 6.0.0 (6.0.21.48005), X64 RyuJIT AVX2
DefaultJob : .NET 6.0.0 (6.0.21.48005), X64 RyuJIT AVX2


 BounceLimit | GridSize | SampleCount |        Mean |      Error |    StdDev |      Median |         Min |         Max | Allocated |
------------ |--------- |------------ |------------:|-----------:|----------:|------------:|------------:|------------:|----------:|
           **1** |        **1** |           **1** |    **47.81 ms** |   **1.906 ms** |  **5.559 ms** |    **46.69 ms** |    **26.87 ms** |    **61.83 ms** |  **14.61 MB** |
           **1** |        **1** |          **10** |    **27.06 ms** |   **0.305 ms** |  **0.285 ms** |    **27.02 ms** |    **26.57 ms** |    **27.54 ms** |  **14.76 MB** |
           **1** |        **1** |         **100** |    **70.16 ms** |   **0.601 ms** |  **0.501 ms** |    **70.12 ms** |    **69.44 ms** |    **71.37 ms** |  **14.61 MB** |
           **1** |        **1** |         **500** |   **284.78 ms** |   **0.751 ms** |  **0.665 ms** |   **284.56 ms** |   **283.89 ms** |   **286.12 ms** |  **14.61 MB** |
           **1** |        **1** |        **1000** |   **563.48 ms** |   **1.591 ms** |  **1.328 ms** |   **563.44 ms** |   **561.50 ms** |   **566.00 ms** |  **16.96 MB** |
           **1** |        **4** |           **1** |    **43.55 ms** |   **5.547 ms** | **16.357 ms** |    **54.25 ms** |    **17.57 ms** |    **59.69 ms** |  **14.63 MB** |
           **1** |        **4** |          **10** |    **22.27 ms** |   **0.690 ms** |  **1.946 ms** |    **22.18 ms** |    **18.20 ms** |    **27.09 ms** |  **14.92 MB** |
           **1** |        **4** |         **100** |    **34.93 ms** |   **0.691 ms** |  **1.174 ms** |    **34.86 ms** |    **32.79 ms** |    **37.32 ms** |  **14.78 MB** |
           **1** |        **4** |         **500** |   **105.04 ms** |   **0.789 ms** |  **0.700 ms** |   **104.90 ms** |   **104.15 ms** |   **106.52 ms** |  **14.15 MB** |
           **1** |        **4** |        **1000** |   **197.27 ms** |   **0.823 ms** |  **0.730 ms** |   **197.32 ms** |   **195.82 ms** |   **198.38 ms** |  **13.84 MB** |
           **1** |        **8** |           **1** |    **22.62 ms** |   **0.442 ms** |  **0.726 ms** |    **22.66 ms** |    **20.02 ms** |    **23.91 ms** |  **14.65 MB** |
           **1** |        **8** |          **10** |    **22.77 ms** |   **0.619 ms** |  **1.825 ms** |    **22.15 ms** |    **19.43 ms** |    **27.10 ms** |  **14.64 MB** |
           **1** |        **8** |         **100** |    **32.16 ms** |   **0.638 ms** |  **0.895 ms** |    **32.09 ms** |    **30.70 ms** |    **34.01 ms** |  **14.94 MB** |
           **1** |        **8** |         **500** |    **91.31 ms** |   **0.958 ms** |  **0.896 ms** |    **91.20 ms** |    **89.83 ms** |    **93.37 ms** |  **14.64 MB** |
           **1** |        **8** |        **1000** |   **162.86 ms** |   **0.408 ms** |  **0.362 ms** |   **162.84 ms** |   **162.07 ms** |   **163.35 ms** |  **14.64 MB** |
           **1** |       **16** |           **1** |    **23.00 ms** |   **0.571 ms** |  **1.657 ms** |    **22.97 ms** |    **17.78 ms** |    **26.35 ms** |  **15.03 MB** |
           **1** |       **16** |          **10** |    **23.10 ms** |   **0.820 ms** |  **2.419 ms** |    **23.24 ms** |    **18.41 ms** |    **27.70 ms** |  **15.03 MB** |
           **1** |       **16** |         **100** |    **31.81 ms** |   **0.626 ms** |  **0.937 ms** |    **31.97 ms** |    **29.87 ms** |    **33.08 ms** |  **14.74 MB** |
           **1** |       **16** |         **500** |    **92.30 ms** |   **1.284 ms** |  **1.201 ms** |    **92.01 ms** |    **89.77 ms** |    **94.27 ms** |  **14.73 MB** |
           **1** |       **16** |        **1000** |   **165.13 ms** |   **0.546 ms** |  **0.427 ms** |   **165.14 ms** |   **164.39 ms** |   **165.71 ms** |  **14.74 MB** |
           **1** |       **32** |           **1** |    **22.52 ms** |   **0.655 ms** |  **1.932 ms** |    **22.97 ms** |    **16.30 ms** |    **25.25 ms** |   **15.1 MB** |
           **1** |       **32** |          **10** |    **22.18 ms** |   **0.629 ms** |  **1.846 ms** |    **22.19 ms** |    **18.90 ms** |    **27.01 ms** |   **15.4 MB** |
           **1** |       **32** |         **100** |    **34.03 ms** |   **0.670 ms** |  **0.939 ms** |    **33.82 ms** |    **32.53 ms** |    **36.22 ms** |  **15.26 MB** |
           **1** |       **32** |         **500** |   **106.48 ms** |   **0.382 ms** |  **0.319 ms** |   **106.53 ms** |   **106.06 ms** |   **106.97 ms** |  **14.64 MB** |
           **1** |       **32** |        **1000** |   **196.99 ms** |   **0.637 ms** |  **0.565 ms** |   **197.02 ms** |   **196.29 ms** |   **197.95 ms** |  **14.31 MB** |
           **1** |       **64** |           **1** |    **19.42 ms** |   **0.767 ms** |  **2.261 ms** |    **18.52 ms** |    **16.17 ms** |    **24.14 ms** |  **16.26 MB** |
           **1** |       **64** |          **10** |    **23.90 ms** |   **0.599 ms** |  **1.758 ms** |    **23.92 ms** |    **18.40 ms** |    **27.47 ms** |  **15.96 MB** |
           **1** |       **64** |         **100** |    **43.89 ms** |   **0.845 ms** |  **1.038 ms** |    **43.78 ms** |    **42.14 ms** |    **45.68 ms** |  **16.95 MB** |
           **1** |       **64** |         **500** |   **143.80 ms** |   **0.626 ms** |  **0.585 ms** |   **143.64 ms** |   **142.99 ms** |   **144.75 ms** |  **15.37 MB** |
           **1** |       **64** |        **1000** |   **276.69 ms** |   **1.065 ms** |  **0.944 ms** |   **276.65 ms** |   **275.08 ms** |   **278.29 ms** |  **16.55 MB** |
           **1** |      **128** |           **1** |    **22.15 ms** |   **0.595 ms** |  **1.728 ms** |    **21.97 ms** |    **19.09 ms** |    **26.84 ms** |  **21.27 MB** |
           **1** |      **128** |          **10** |    **25.36 ms** |   **0.387 ms** |  **0.323 ms** |    **25.34 ms** |    **24.74 ms** |    **25.82 ms** |  **20.98 MB** |
           **1** |      **128** |         **100** |    **55.75 ms** |   **0.450 ms** |  **0.399 ms** |    **55.76 ms** |    **55.15 ms** |    **56.43 ms** |  **21.83 MB** |
           **1** |      **128** |         **500** |   **206.15 ms** |   **0.825 ms** |  **0.731 ms** |   **205.85 ms** |   **205.25 ms** |   **207.48 ms** |  **21.51 MB** |
           **1** |      **128** |        **1000** |   **403.22 ms** |   **1.709 ms** |  **1.599 ms** |   **403.57 ms** |   **400.70 ms** |   **405.86 ms** |  **24.65 MB** |
           **4** |        **1** |           **1** |    **23.79 ms** |   **0.471 ms** |  **1.290 ms** |    **23.76 ms** |    **21.04 ms** |    **26.97 ms** |  **14.47 MB** |
           **4** |        **1** |          **10** |    **38.72 ms** |   **0.612 ms** |  **0.573 ms** |    **38.86 ms** |    **37.71 ms** |    **39.48 ms** |  **14.79 MB** |
           **4** |        **1** |         **100** |   **216.35 ms** |   **0.389 ms** |  **0.345 ms** |   **216.42 ms** |   **215.65 ms** |   **216.95 ms** |  **13.83 MB** |
           **4** |        **1** |         **500** | **1,040.81 ms** |   **3.641 ms** |  **3.040 ms** | **1,041.27 ms** | **1,036.33 ms** | **1,046.85 ms** |  **16.96 MB** |
           **4** |        **1** |        **1000** | **2,066.91 ms** |   **1.552 ms** |  **1.296 ms** | **2,066.64 ms** | **2,064.03 ms** | **2,068.72 ms** |  **16.96 MB** |
           **4** |        **4** |           **1** |    **23.03 ms** |   **0.825 ms** |  **2.432 ms** |    **23.53 ms** |    **16.92 ms** |    **27.77 ms** |  **14.77 MB** |
           **4** |        **4** |          **10** |    **27.05 ms** |   **0.334 ms** |  **0.296 ms** |    **27.11 ms** |    **26.50 ms** |    **27.36 ms** |  **14.62 MB** |
           **4** |        **4** |         **100** |   **104.49 ms** |   **0.703 ms** |  **0.657 ms** |   **104.69 ms** |   **103.58 ms** |   **105.29 ms** |  **15.09 MB** |
           **4** |        **4** |         **500** |   **461.11 ms** |   **7.379 ms** | **14.565 ms** |   **457.82 ms** |   **441.66 ms** |   **508.42 ms** |  **16.98 MB** |
           **4** |        **4** |        **1000** |   **896.88 ms** |   **0.754 ms** |  **0.630 ms** |   **896.98 ms** |   **895.74 ms** |   **897.99 ms** |  **16.97 MB** |
           **4** |        **8** |           **1** |    **22.09 ms** |   **0.826 ms** |  **2.435 ms** |    **21.63 ms** |    **18.07 ms** |    **27.57 ms** |  **14.65 MB** |
           **4** |        **8** |          **10** |    **26.51 ms** |   **0.474 ms** |  **0.443 ms** |    **26.57 ms** |    **25.83 ms** |    **27.44 ms** |  **14.64 MB** |
           **4** |        **8** |         **100** |    **87.40 ms** |   **0.743 ms** |  **0.620 ms** |    **87.22 ms** |    **86.39 ms** |    **88.29 ms** |  **14.65 MB** |
           **4** |        **8** |         **500** |   **367.34 ms** |   **1.145 ms** |  **1.071 ms** |   **367.16 ms** |   **365.57 ms** |   **369.49 ms** |  **16.99 MB** |
           **4** |        **8** |        **1000** |   **718.21 ms** |   **3.247 ms** |  **2.878 ms** |   **718.42 ms** |   **714.26 ms** |   **723.06 ms** |  **16.99 MB** |
           **4** |       **16** |           **1** |    **23.86 ms** |   **0.781 ms** |  **2.302 ms** |    **23.55 ms** |    **18.52 ms** |    **28.56 ms** |  **14.59 MB** |
           **4** |       **16** |          **10** |    **28.46 ms** |   **0.567 ms** |  **0.814 ms** |    **28.38 ms** |    **27.17 ms** |    **30.25 ms** |  **14.89 MB** |
           **4** |       **16** |         **100** |    **87.06 ms** |   **1.466 ms** |  **1.300 ms** |    **86.89 ms** |    **85.34 ms** |    **89.44 ms** |  **14.74 MB** |
           **4** |       **16** |         **500** |   **365.84 ms** |   **1.510 ms** |  **1.339 ms** |   **365.26 ms** |   **364.42 ms** |   **368.03 ms** |  **17.09 MB** |
           **4** |       **16** |        **1000** |   **712.21 ms** |   **1.691 ms** |  **1.582 ms** |   **712.17 ms** |   **708.94 ms** |   **714.58 ms** |   **17.1 MB** |
           **4** |       **32** |           **1** |    **23.06 ms** |   **0.911 ms** |  **2.685 ms** |    **22.62 ms** |    **18.61 ms** |    **29.71 ms** |  **14.96 MB** |
           **4** |       **32** |          **10** |    **29.76 ms** |   **0.587 ms** |  **1.171 ms** |    **30.14 ms** |    **27.35 ms** |    **32.01 ms** |   **15.1 MB** |
           **4** |       **32** |         **100** |    **98.19 ms** |   **1.942 ms** |  **2.907 ms** |    **98.18 ms** |    **93.43 ms** |   **104.88 ms** |  **15.58 MB** |
           **4** |       **32** |         **500** |   **422.97 ms** |   **8.278 ms** | **13.831 ms** |   **416.07 ms** |   **410.37 ms** |   **451.89 ms** |  **17.45 MB** |
           **4** |       **32** |        **1000** |   **852.92 ms** |  **16.949 ms** | **32.247 ms** |   **845.21 ms** |   **808.48 ms** |   **931.76 ms** |  **17.46 MB** |
           **4** |       **64** |           **1** |    **22.32 ms** |   **0.446 ms** |  **1.228 ms** |    **22.14 ms** |    **18.93 ms** |    **25.40 ms** |  **16.26 MB** |
           **4** |       **64** |          **10** |    **30.40 ms** |   **0.599 ms** |  **0.915 ms** |    **30.60 ms** |    **27.72 ms** |    **31.72 ms** |   **16.4 MB** |
           **4** |       **64** |         **100** |   **125.07 ms** |   **2.424 ms** |  **2.490 ms** |   **125.59 ms** |   **122.17 ms** |   **128.93 ms** |  **16.08 MB** |
           **4** |       **64** |         **500** |   **554.34 ms** |   **1.498 ms** |  **1.401 ms** |   **553.92 ms** |   **551.67 ms** |   **557.42 ms** |  **18.91 MB** |
           **4** |       **64** |        **1000** | **1,087.00 ms** |   **1.461 ms** |  **1.295 ms** | **1,087.19 ms** | **1,084.45 ms** | **1,089.15 ms** |  **18.91 MB** |
           **4** |      **128** |           **1** |    **24.08 ms** |   **0.479 ms** |  **1.304 ms** |    **24.03 ms** |    **21.31 ms** |    **27.73 ms** |  **20.98 MB** |
           **4** |      **128** |          **10** |    **36.10 ms** |   **0.703 ms** |  **0.889 ms** |    **36.09 ms** |    **34.63 ms** |    **38.02 ms** |  **22.15 MB** |
           **4** |      **128** |         **100** |   **173.94 ms** |   **0.867 ms** |  **0.768 ms** |   **173.88 ms** |   **172.24 ms** |   **175.30 ms** |  **21.52 MB** |
           **4** |      **128** |         **500** |   **809.45 ms** |   **2.075 ms** |  **1.840 ms** |   **809.03 ms** |   **806.89 ms** |   **813.18 ms** |  **24.65 MB** |
           **4** |      **128** |        **1000** | **1,593.56 ms** |   **2.816 ms** |  **2.634 ms** | **1,593.51 ms** | **1,589.66 ms** | **1,598.18 ms** |  **24.65 MB** |
           **8** |        **1** |           **1** |    **27.09 ms** |   **0.518 ms** |  **0.485 ms** |    **27.13 ms** |    **26.03 ms** |    **27.99 ms** |  **14.62 MB** |
           **8** |        **1** |          **10** |    **56.15 ms** |   **1.123 ms** |  **1.050 ms** |    **56.03 ms** |    **54.20 ms** |    **57.98 ms** |  **14.88 MB** |
           **8** |        **1** |         **100** |   **414.59 ms** |   **1.334 ms** |  **1.114 ms** |   **414.34 ms** |   **413.09 ms** |   **417.33 ms** |  **16.97 MB** |
           **8** |        **1** |         **500** | **2,010.53 ms** |   **4.055 ms** |  **3.793 ms** | **2,009.38 ms** | **2,005.62 ms** | **2,019.74 ms** |  **16.97 MB** |
           **8** |        **1** |        **1000** | **4,002.45 ms** |   **3.618 ms** |  **3.021 ms** | **4,001.15 ms** | **3,998.94 ms** | **4,008.85 ms** |  **16.97 MB** |
           **8** |        **4** |           **1** |    **23.78 ms** |   **0.470 ms** |  **1.285 ms** |    **23.56 ms** |    **21.07 ms** |    **26.89 ms** |  **14.77 MB** |
           **8** |        **4** |          **10** |    **36.60 ms** |   **0.725 ms** |  **0.890 ms** |    **36.46 ms** |    **35.15 ms** |    **38.48 ms** |  **14.63 MB** |
           **8** |        **4** |         **100** |   **193.70 ms** |   **0.622 ms** |  **0.582 ms** |   **193.83 ms** |   **192.67 ms** |   **194.47 ms** |  **13.83 MB** |
           **8** |        **4** |         **500** |   **922.45 ms** |   **2.907 ms** |  **2.719 ms** |   **921.35 ms** |   **917.95 ms** |   **927.18 ms** |  **16.99 MB** |
           **8** |        **4** |        **1000** | **1,834.80 ms** |   **2.433 ms** |  **2.157 ms** | **1,834.44 ms** | **1,831.11 ms** | **1,839.25 ms** |  **16.98 MB** |
           **8** |        **8** |           **1** |    **23.06 ms** |   **0.533 ms** |  **1.573 ms** |    **22.91 ms** |    **19.43 ms** |    **27.20 ms** |  **14.65 MB** |
           **8** |        **8** |          **10** |    **33.27 ms** |   **0.654 ms** |  **0.727 ms** |    **33.29 ms** |    **31.77 ms** |    **34.79 ms** |  **14.64 MB** |
           **8** |        **8** |         **100** |   **158.84 ms** |   **0.613 ms** |  **0.574 ms** |   **158.67 ms** |   **158.08 ms** |   **159.96 ms** |  **14.64 MB** |
           **8** |        **8** |         **500** |   **732.70 ms** |   **1.839 ms** |  **1.720 ms** |   **732.47 ms** |   **729.90 ms** |   **736.35 ms** |  **16.99 MB** |
           **8** |        **8** |        **1000** | **1,453.24 ms** |   **6.762 ms** |  **6.325 ms** | **1,455.06 ms** | **1,442.40 ms** | **1,461.78 ms** |  **16.99 MB** |
           **8** |       **16** |           **1** |    **24.16 ms** |   **0.612 ms** |  **1.786 ms** |    **23.85 ms** |    **19.97 ms** |    **28.46 ms** |  **14.59 MB** |
           **8** |       **16** |          **10** |    **33.72 ms** |   **0.636 ms** |  **0.624 ms** |    **33.96 ms** |    **32.54 ms** |    **34.51 ms** |  **14.74 MB** |
           **8** |       **16** |         **100** |   **154.37 ms** |   **1.509 ms** |  **1.411 ms** |   **154.16 ms** |   **152.06 ms** |   **157.43 ms** |  **14.74 MB** |
           **8** |       **16** |         **500** |   **708.14 ms** |   **1.813 ms** |  **1.607 ms** |   **707.85 ms** |   **706.53 ms** |   **711.45 ms** |   **17.1 MB** |
           **8** |       **16** |        **1000** | **1,398.65 ms** |   **2.223 ms** |  **1.970 ms** | **1,398.61 ms** | **1,395.25 ms** | **1,401.99 ms** |  **17.09 MB** |
           **8** |       **32** |           **1** |    **22.66 ms** |   **0.533 ms** |  **1.547 ms** |    **22.76 ms** |    **19.22 ms** |    **25.92 ms** |   **15.4 MB** |
           **8** |       **32** |          **10** |    **35.86 ms** |   **0.756 ms** |  **2.218 ms** |    **35.46 ms** |    **31.12 ms** |    **41.95 ms** |  **15.26 MB** |
           **8** |       **32** |         **100** |   **169.50 ms** |   **0.896 ms** |  **0.794 ms** |   **169.49 ms** |   **168.46 ms** |   **171.13 ms** |  **14.31 MB** |
           **8** |       **32** |         **500** |   **810.10 ms** |  **12.412 ms** | **11.610 ms** |   **803.95 ms** |   **796.82 ms** |   **830.83 ms** |  **17.46 MB** |
           **8** |       **32** |        **1000** | **1,579.65 ms** |  **21.150 ms** | **40.749 ms** | **1,568.23 ms** | **1,557.61 ms** | **1,749.78 ms** |  **17.45 MB** |
           **8** |       **64** |           **1** |    **22.63 ms** |   **0.529 ms** |  **1.560 ms** |    **22.64 ms** |    **19.57 ms** |    **25.88 ms** |  **15.96 MB** |
           **8** |       **64** |          **10** |    **38.04 ms** |   **0.756 ms** |  **1.324 ms** |    **38.12 ms** |    **35.24 ms** |    **40.67 ms** |  **16.21 MB** |
           **8** |       **64** |         **100** |   **216.75 ms** |   **0.639 ms** |  **0.567 ms** |   **216.71 ms** |   **215.68 ms** |   **217.53 ms** |  **15.76 MB** |
           **8** |       **64** |         **500** | **1,035.15 ms** |   **1.167 ms** |  **1.092 ms** | **1,035.18 ms** | **1,032.89 ms** | **1,037.09 ms** |  **18.89 MB** |
           **8** |       **64** |        **1000** | **2,064.88 ms** |   **3.322 ms** |  **3.107 ms** | **2,064.30 ms** | **2,059.78 ms** | **2,071.15 ms** |  **18.91 MB** |
           **8** |      **128** |           **1** |    **25.06 ms** |   **0.497 ms** |  **0.729 ms** |    **24.76 ms** |    **24.13 ms** |    **26.79 ms** |  **22.15 MB** |
           **8** |      **128** |          **10** |    **49.14 ms** |   **0.908 ms** |  **0.849 ms** |    **49.35 ms** |    **47.34 ms** |    **50.27 ms** |  **22.08 MB** |
           **8** |      **128** |         **100** |   **315.82 ms** |   **1.126 ms** |  **1.053 ms** |   **315.75 ms** |   **313.54 ms** |   **317.98 ms** |   **22.3 MB** |
           **8** |      **128** |         **500** | **1,515.83 ms** |   **4.862 ms** |  **4.310 ms** | **1,517.32 ms** | **1,507.20 ms** | **1,522.64 ms** |  **24.64 MB** |
           **8** |      **128** |        **1000** | **2,999.09 ms** |   **2.454 ms** |  **2.049 ms** | **2,998.81 ms** | **2,995.79 ms** | **3,002.75 ms** |  **24.65 MB** |
          **16** |        **1** |           **1** |    **26.86 ms** |   **0.523 ms** |  **0.603 ms** |    **27.01 ms** |    **25.49 ms** |    **27.62 ms** |  **14.76 MB** |
          **16** |        **1** |          **10** |    **93.18 ms** |   **1.108 ms** |  **1.037 ms** |    **93.38 ms** |    **91.45 ms** |    **94.53 ms** |  **14.61 MB** |
          **16** |        **1** |         **100** |   **784.99 ms** |   **1.510 ms** |  **1.413 ms** |   **784.84 ms** |   **782.88 ms** |   **787.27 ms** |  **16.96 MB** |
          **16** |        **1** |         **500** | **3,850.49 ms** |   **2.780 ms** |  **2.322 ms** | **3,850.59 ms** | **3,846.81 ms** | **3,853.75 ms** |  **16.95 MB** |
          **16** |        **1** |        **1000** | **7,676.46 ms** |   **6.296 ms** |  **5.890 ms** | **7,675.41 ms** | **7,669.86 ms** | **7,689.40 ms** |  **16.96 MB** |
          **16** |        **4** |           **1** |    **25.25 ms** |   **0.503 ms** |  **0.956 ms** |    **25.39 ms** |    **23.83 ms** |    **27.45 ms** |  **14.48 MB** |
          **16** |        **4** |          **10** |    **51.37 ms** |   **0.547 ms** |  **0.485 ms** |    **51.25 ms** |    **50.48 ms** |    **52.06 ms** |  **14.62 MB** |
          **16** |        **4** |         **100** |   **367.58 ms** |   **0.854 ms** |  **0.799 ms** |   **367.40 ms** |   **366.23 ms** |   **369.02 ms** |  **16.98 MB** |
          **16** |        **4** |         **500** | **1,777.12 ms** |   **2.040 ms** |  **1.908 ms** | **1,776.65 ms** | **1,773.89 ms** | **1,780.91 ms** |  **16.96 MB** |
          **16** |        **4** |        **1000** | **3,534.75 ms** |   **3.243 ms** |  **2.708 ms** | **3,534.80 ms** | **3,527.63 ms** | **3,538.24 ms** |  **16.97 MB** |
          **16** |        **8** |           **1** |    **23.62 ms** |   **0.468 ms** |  **0.714 ms** |    **23.68 ms** |    **22.54 ms** |    **25.09 ms** |  **14.65 MB** |
          **16** |        **8** |          **10** |    **44.24 ms** |   **0.600 ms** |  **0.532 ms** |    **44.12 ms** |    **43.52 ms** |    **45.36 ms** |  **14.64 MB** |
          **16** |        **8** |         **100** |   **289.96 ms** |   **1.073 ms** |  **1.004 ms** |   **290.04 ms** |   **288.33 ms** |   **291.14 ms** |  **14.64 MB** |
          **16** |        **8** |         **500** | **1,399.79 ms** |   **1.151 ms** |  **1.021 ms** | **1,399.74 ms** | **1,397.93 ms** | **1,401.31 ms** |  **17.01 MB** |
          **16** |        **8** |        **1000** | **2,779.55 ms** |   **2.115 ms** |  **1.766 ms** | **2,779.96 ms** | **2,776.12 ms** | **2,782.68 ms** |  **16.99 MB** |
          **16** |       **16** |           **1** |    **24.73 ms** |   **0.490 ms** |  **0.819 ms** |    **24.80 ms** |    **22.82 ms** |    **26.22 ms** |  **14.88 MB** |
          **16** |       **16** |          **10** |    **42.73 ms** |   **0.841 ms** |  **1.179 ms** |    **42.56 ms** |    **41.02 ms** |    **44.55 ms** |  **15.13 MB** |
          **16** |       **16** |         **100** |   **274.90 ms** |   **1.397 ms** |  **1.306 ms** |   **274.96 ms** |   **272.69 ms** |   **277.01 ms** |  **14.73 MB** |
          **16** |       **16** |         **500** | **1,324.62 ms** |   **1.325 ms** |  **1.239 ms** | **1,324.44 ms** | **1,322.70 ms** | **1,326.62 ms** |  **17.08 MB** |
          **16** |       **16** |        **1000** | **2,630.72 ms** |   **2.519 ms** |  **2.356 ms** | **2,631.13 ms** | **2,626.46 ms** | **2,635.13 ms** |  **17.09 MB** |
          **16** |       **32** |           **1** |    **24.95 ms** |   **0.500 ms** |  **1.476 ms** |    **25.11 ms** |    **20.62 ms** |    **28.58 ms** |  **15.25 MB** |
          **16** |       **32** |          **10** |    **46.63 ms** |   **0.921 ms** |  **1.261 ms** |    **46.74 ms** |    **44.92 ms** |    **49.49 ms** |   **15.1 MB** |
          **16** |       **32** |         **100** |   **310.94 ms** |   **0.471 ms** |  **0.441 ms** |   **310.98 ms** |   **310.04 ms** |   **311.60 ms** |   **15.1 MB** |
          **16** |       **32** |         **500** | **1,512.53 ms** |   **2.382 ms** |  **1.860 ms** | **1,512.47 ms** | **1,509.69 ms** | **1,516.59 ms** |  **17.45 MB** |
          **16** |       **32** |        **1000** | **3,056.41 ms** |  **24.733 ms** | **19.310 ms** | **3,063.54 ms** | **3,017.61 ms** | **3,076.59 ms** |  **17.45 MB** |
          **16** |       **64** |           **1** |    **25.85 ms** |   **0.546 ms** |  **1.593 ms** |    **25.91 ms** |    **21.88 ms** |    **29.43 ms** |  **15.96 MB** |
          **16** |       **64** |          **10** |    **59.51 ms** |   **1.165 ms** |  **1.295 ms** |    **59.49 ms** |    **56.46 ms** |    **61.88 ms** |  **16.81 MB** |
          **16** |       **64** |         **100** |   **420.35 ms** |   **2.026 ms** |  **1.796 ms** |   **419.98 ms** |   **417.48 ms** |   **424.18 ms** |   **18.9 MB** |
          **16** |       **64** |         **500** | **2,083.39 ms** |  **11.932 ms** |  **9.315 ms** | **2,080.79 ms** | **2,076.33 ms** | **2,111.65 ms** |   **18.9 MB** |
          **16** |       **64** |        **1000** | **4,125.98 ms** |  **27.491 ms** | **22.956 ms** | **4,134.99 ms** | **4,098.78 ms** | **4,172.27 ms** |   **18.9 MB** |
          **16** |      **128** |           **1** |    **26.16 ms** |   **0.500 ms** |  **0.556 ms** |    **26.09 ms** |    **25.29 ms** |    **27.55 ms** |  **21.42 MB** |
          **16** |      **128** |          **10** |    **80.02 ms** |   **1.571 ms** |  **1.987 ms** |    **79.77 ms** |    **75.83 ms** |    **83.50 ms** |  **21.97 MB** |
          **16** |      **128** |         **100** |   **622.63 ms** |   **3.089 ms** |  **2.580 ms** |   **622.98 ms** |   **619.27 ms** |   **627.68 ms** |  **24.66 MB** |
          **16** |      **128** |         **500** | **3,025.11 ms** |   **2.891 ms** |  **2.414 ms** | **3,025.12 ms** | **3,020.34 ms** | **3,028.25 ms** |  **24.65 MB** |
          **16** |      **128** |        **1000** | **6,054.09 ms** | **111.278 ms** | **92.922 ms** | **6,024.74 ms** | **5,875.02 ms** | **6,276.64 ms** |  **24.64 MB** |
