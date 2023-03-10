# Welcome to FitnessVR!
## Steps to run FitnessVR's real-time workout and repetition detection:
### 1. From the root of the repository run `pip3 install -r requirements.txt`
### 2. You will need a computer and the Oculus to be on the same network.
### 3. You will need to change the IP address in our scripts to the IP of your Oculus.
You can find the IP of your Oculus by following the instructions [here](https://www.youtube.com/watch?v=gL1vgWubcJw&ab_channel=VRLad). Replace the IP on line 13 of FitnessVRDetector.cs with the IP of your Oculus. Additionally, replace the IP on line 31 of server.py with the IP of your Oculus. **You should be modifying 1 line of code in 2 files.**
### 4. Build and run the Unity scene called "FitnessVR Detection" on the Oculus. You should see a black Unity loading screen.
### 5. From the root of the repository, run `python3 .\Python\FitnessVR\server.py`.
If connected successfully, you should see "Connected to Oculus" printed in the terminal and the Unity scene should display your repetitions and either "curl" or "jumping jacks."
