import os
import pandas as pd
import shutil

directory = "../../Data/FitnessVR/Labeled/"
output_directory = "../../Data/FitnessVR/Cleaned/"

def clearFolder():
    if os.path.exists(output_directory):
        for filename in os.listdir(output_directory):
            file_path = os.path.join(output_directory, filename)
            try:
                if os.path.isfile(file_path) or os.path.islink(file_path):
                    os.unlink(file_path)
                elif os.path.isdir(file_path):
                    shutil.rmtree(file_path)
            except Exception as e:
                print('Failed to delete %s. Reason: %s' % (file_path, e))

clearFolder()
for filename in os.listdir(directory):
    if filename.endswith('.csv'):
        df = pd.read_csv(os.path.join(directory, filename))
        df = df.iloc[:719]
        # Save the cropped dataframe to a new CSV file
        output_filename = os.path.join(output_directory, filename)
        df.to_csv(output_filename)