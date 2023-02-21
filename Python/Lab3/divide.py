import os, shutil, random
directory = "../../Data/Lab3/"
testing_directory = "../../Data/Lab3/Test/"
train_directory = "../../Data/Lab3/Train/"

ratio = 0.8

male_set = (20, 21, 22, 23, 24, 26, 28, 38)
female_set = (25, 30, 39, 40, 42, 44, 48)

num = 0
if os.path.exists(directory):
    for filename in os.listdir(directory):
        if filename[-3:] != "csv":
            continue
        file_name = ""
        person = int(int(''.join(c for c in filename[6:8] if c.isdigit())))
        if person in male_set:
            file_name =  ("M" + str(num))
        else:
            file_name = ("F" + str(num))
        num += 1
        file_path = os.path.join(directory, filename)
        print(file_name)
        # RNG, determine training set
        if(random.random() <= ratio):
            shutil.copy2(file_path, train_directory + file_name)
        else:
            shutil.copy2(file_path, testing_directory + file_name)