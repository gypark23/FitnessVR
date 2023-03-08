import socket
import random
import pandas as pd
#import time_series as TS
import numpy as np

col_names = ['time', 'headset_vel.x', 'headset_vel.y', 'headset_vel.z', 'headset_angularVel.x',
             'headset_angularVel.y', 'headset_angularVel.z', 'headset_pos.x', 'headset_pos.y',
             'headset_pos.z', 'headset_rot.x', 'headset_rot.y', 'headset_rot.z', 'controller_left_vel.x',
             'controller_left_vel.y', 'controller_left_vel.z', 'controller_left_angularVel.x',
             'controller_left_angularVel.y', 'controller_left_angularVel.z', 'controller_left_pos.x',
             'controller_left_pos.y', 'controller_left_pos.z', 'controller_left_rot.x', 'controller_left_rot.y',
             'controller_left_rot.z', 'controller_right_vel.x', 'controller_right_vel.y',
             'controller_right_vel.z', 'controller_right_angularVel.x', 'controller_right_angularVel.y',
             'controller_right_angularVel.z', 'controller_right_pos.x', 'controller_right_pos.y',
             'controller_right_pos.z', 'controller_right_rot.x', 'controller_right_rot.y',
             'controller_right_rot.z']

def string_to_dataframe(s):
    # Convert the string to a list of lists of floats
    list_of_lists = [[float(num) for num in line.split()] for line in s.strip().split('\n')]
    df = pd.DataFrame(data=list_of_lists).T
    df.columns = col_names
    
    return df

# print("Deep Learning Model Constructing...")
# model = TS.create_LSTM()
# print("Model Created")

host, port = "192.168.1.22", 25001
sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
sock.connect((host, port))


while True:
    data = sock.recv(1024)
    if data:
        # convert byte array to string and then dataframe
        string_data = data.decode()
        df_data = string_to_dataframe(string_data)
        print(df_data)

        # #DF preprocessing for Deep Learning Model
        # X = df_data.values
        # timestep = 1 #
        # num_samples = X.shape[0] // timestep
        # num_timesteps = timestep
        # num_features = X.shape[1]
        # X = X[:num_samples * timestep].reshape(num_samples, num_timesteps, num_features)

        # #Predict
        # y_pred = np.around(model.predict(X)).astype(int)
        
        # if y_pred[0] == 0:
        #         prediction = "curl"
        # else:
        #         prediction = "jumping jacks"
        num = random.randint(0,1)
        if num == 0:
            prediction = "curl"
        else:
            prediction = "jumping jack"
        sock.sendall(prediction.encode("UTF-8")) #Converting string to Byte, and sending it to C#