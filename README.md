# Car_ML-Agent_in_Unity
This project was created during my university's course "INTELLIGENT AGENTS"<br>
<br>
An enviroment was designed in unity for a driving car and custom physics were created to control this car.<br>
Transparent walls were added to represent borders around the street while pass-through walls were added inside the street to act as checkpoints<br>
<br>
Unity ML-Agents was used to train the car using the custom physics. When the car hits the red walls around the street, it receives negative reward. When the car hits the next checkpoint, it receives positive reward.<br>
The ML_Agent uses ray-casts to perform observations. It learns using both reinforcement learning and imitation learning.<br>
<br>
The .onnx file inside assets folder contains the trained ml-agent.
