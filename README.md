Mouse AI Project

    Mouse artificial intelligence project created for UWGB CS 464 Fall semester, 2020.

Applies artificial intelligence concepts towards simulation of a biological mouse which traverses
a maze by memory for a cheese reward. The virtual mouse traverses a collection of randomly 
generated mazes utilizing a sense of sight, smell and neural memory feedback. 

The program creates configurable, trained neural network models to the individual maze paths that 
the mouse searches by sight to reach the cheese goal. The trained models then provide prediction 
feedback of neural path memories via visual recognition as the mouse repeatedly traverses the mazes. 
A sense of smell is also simulated via logic decision processing by extending cheese detection at a 
given distance in all possible directions which leads the mouse to the cheese goal.

Development setup

    C# Visual Studio Community 2019
    x64 build
    Python 3.8x installation with Keras, Numpy and TensorFlow libraries:
        Python dependencies:
            pip install keras
            pip install numpy
            pip install tensorflow

    The startup project is MouseAI.UI

Usage example

    From the menu a new project can be created with selected random mazes or loaded.
    Process -> Path iterates the path solving and segment building for all mazes (automatic on project creation)
    Process -> Train performs neural model configuration and training
    Model results can be saved
    Process -> Test performs neural model selections
    Prediction testing
    Run testing
    Model information and data exporting as csv file

    Notes:
    A Python path and library dependency checker runs on startup due to the .NET implementations; the specific 
    Python version and libraries are required or they will crash with no warning. The invoked Python will 
    occasionally not respond to commands after repeated calls in a multithreaded environment; the current fix 
    is to restart the application (the app automatically restarts after training).
    A SQLite database table tracks logs and model creation between projects; the dependency “SQLite.Interop.dll” 
    x64 has been added to the directory ~\MouseAI.UI\ to assist with building.

Release History

    0.0.2
        Readme work in progress

    0.0.1
        Work in progress

Meta

Nathaniel Kennis kennnl04@uwgb.edu

Distributed under the Apache license. See LICENSE for more information.

https://github.com/drumzzzzz/MazeAI


Contributing

    Fork it (https://github.com/drumzzzzz/MazeAI)
    Create a new Pull Request
