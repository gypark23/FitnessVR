import socket

import pandas as pd
from io import StringIO

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


server_address = ('10.0.0.140', 1234)
server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
server_socket.bind(server_address)
server_socket.listen(1)


print("Waiting for connection...")
client_socket, client_address = server_socket.accept()
print(f"Connected to {client_address}")
    
received_data = b''
remaining_bytes = 17279
while remaining_bytes > 0:
    chunk_size = min(remaining_bytes, 1024)
    data = client_socket.recv(chunk_size)
    if not data:
        break  # the socket has been closed by the other end
    received_data += data
    remaining_bytes -= len(data)


    
# Convert the byte array to a string and print it
string_data = received_data.decode()
print(string_data)
df_data = string_to_dataframe(string_data)
print(df_data)
        

        
# client_socket.close()
# server_socket.close()
    
    
