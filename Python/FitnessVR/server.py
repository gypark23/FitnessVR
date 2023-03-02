import socket

server_address = ('10.150.10.209', 1234)
server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
server_socket.bind(server_address)
server_socket.listen(1)

while True:
    print("Waiting for connection...")
    client_socket, client_address = server_socket.accept()
    print(f"Connected to {client_address}")
    
    data = client_socket.recv(1024)
    if data:
        # Convert the byte array to a string and print it
        print(data.decode())
        
    client_socket.close()