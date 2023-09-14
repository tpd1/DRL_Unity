
# Deep Reinforcement Learning for AI Navigation in Video Games

#### -- Author: Tim Deville
#### -- Project Status: [Completed]
#### -- Video Demonstration: TBC
#### -- [Project Poster](/ProjectPoster.png "Project Poster")

## Project Intro/Objective
This project aims to create a generalised AI agent using deep reinforcement learning that is capable of nagivating complex procedurally generated indoor environments. The goal of this project is to explore the potential benefits of deploying such an agent into modern video games, improving the behavioural characteristics and self-navigtation ability of video game NPCs.

### Acknowledgements
* Submitted as part of a Computer Science BSc dissertation at Swansea University, UK.
* Dissertation Supervisor: Dr Mike Edwards.

### Methods Used
* Deep Reinforcement Learning
* Video Game Design
* Procedural Generation
* Data Visualisation

### Technologies
* Python
* C#
* Unity
* Pandas, Matplotlib, Jupyter
* PyTorch

## Project Description

The navigation ability of current video game AI characters relies predominantly on pre-programmed rule-based methods that can lead to labour-intensive development and produce AI that exhibit predictable and unengaging behaviour. This project proposes Deep Reinforcement Learning (DRL) as an alternative to traditional methods of AI navigation in video games. This project builds upon previous studies involving AI navigation in static environments and aims to assess whether a more generalised DRL agent can be trained using procedurally generated environments and curriculum learning.

## Assets Used

The following licenced assets were used with permission and are required in order to re-create this project:

- Synty Studios - Polygon Asset pack.
- Synty Studios - Polygon Prototype pack.

### Scripts

The function of individual scripts can be read at the start of each C# file. The folder structure of this submission is given below:

* Agent Scripts - All scripts related to the agent's functionality, including variations of the agent script used for training, testing & a demonstration.
* Envrionment Generation Scripts - Scripts containing features such as the procedural generation algorithm and doorway activation functions.
* Other - Scripts such as UI & camera control.
* Testing Scripts - All scripts used during the testing process, such as the position & stats loggers and line drawing component. 

## Data

Also included in this repository are the following files containing project data:

* Agent Path JSON Files - Agent co-ordinates for each test level collected by the position logger. These were used to render the lines displayed in the Path Analysis section.
* NN Test Configurations - Files generated from the ANN size experiment that show evidence of the training process and configuration for each test.
* Path Analysis Hi-res Images - 4K path analysis images that match those presented in the Path Analysis section of the document.
* Raw Training Data - Training data exported from Tensorboard.
* Static Layout Test Results - Results manually logged from the static layout tests. 
* Jupyter Scripts - Jupyter Notebook scripts that were used to visualise the Tensorboard data for the document.

Tensorboard training results for the NN size experiment can be viewed at:
https://tensorboard.dev/experiment/gk3P7BBlSZy05W2ajZoiow/#scalars


## Installation

A brief overview of how this project was created is provided in following steps:

1. Create a 3D unity project and install the ML-agents Unity toolkit.
2. Create a Python virtual environment within the project root folder.
3. Within this envrionment install the ML-agents python package, PyTorch and related dependencies.
4. If using an Nvidia GPU, also install CUDA drivers and CuDNN neural network libraries.

## Envrionment & Agent Creation

1. Within Unity, create the room prefabs and tag Wall, Obstacle and Doorway components. Attach PropGenerator & SmokeSpawner scripts to the room. 
2. Create doorways with box colliders that connect each room, attaching the Doorway script to each.
3. Create game objects for the singleton classes LevelParameters, RoomPrefabs & AgentManager.
4. Create a player character and attach the DAgent script, followed by the custom animation and movement scripts.
5. Add the main FloorGenerator script to an empty game object.
   
## Training Setup

1. Open the command line terminal in the project's root folder.
2. Activate the python virtual envrionment: ```venv\Scripts\activate```
3. With the Unity window also open and the envrionment set up, begin the ML-agents training process from the command line: ```mlagents-learn config.yaml --run-id=Test1```, where config.yaml is the hyperparameter configuration file and run-id is the run identifier.
4. Press play in the Unity editor to start the training process.
5. After training, Tensorboard results can be viewed using the command: ```tensorboard --logdir results --port 6006``` and opening a web browser at that port.

## Testing
To test the agent using the built in inference engine in Unity, first copy the .onyx file created during the training process to the project's assets folder. Inside Unity, select the agent and change its mode to Inference only and attach the .onyx brain.

## Contact
* [![Linkedin]][Linkedin-url]
* Email: tim.deville2@gmail.com

[Linkedin]: https://img.shields.io/badge/linkedin-%230077B5.svg?style=for-the-badge&logo=linkedin&logoColor=white
[Linkedin-url]: https://www.linkedin.com/in/tim-deville/
