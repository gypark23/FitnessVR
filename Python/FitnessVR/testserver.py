import socket
import random

host, port = "192.168.1.22", 25001
sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
sock.connect((host, port))


while True:
    data = sock.recv(1024)
    if data:
        string_data = data.decode()
        print(string_data)
        num = random.randint(0,1)
        if num == 0:
            exercise = "curl"
        else:
            exercise = "jumping jack"
        sock.sendall(exercise.encode("UTF-8")) #Converting string to Byte, and sending it to C#