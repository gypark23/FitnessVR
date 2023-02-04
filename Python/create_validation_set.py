"""

Write code that takes the labeled data and outputs a training dataset in `Data/Lab2/Train/`
and a validation dataset in `Data/Lab2/Validation`. These datasets should contain CSV files
that have their corresponding activity in their filename (similar to the the `Labeled/` folder).

No file should appear in both the training and validation sets.

Be sure to include documentation with clear instructions on how to run the script and expected outputs.

"""
import os, shutil, random

# Helper function to clear existing files in Train and Validation Folders
def clearFolder():
    directory = "../Data/Lab2/Validation"
    directory_t = "../Data/Lab2/Train"
    if os.path.exists(directory):
        for filename in os.listdir(directory):
            file_path = os.path.join(directory, filename)
            try:
                if os.path.isfile(file_path) or os.path.islink(file_path):
                    os.unlink(file_path)
                elif os.path.isdir(file_path):
                    shutil.rmtree(file_path)
            except Exception as e:
                print('Failed to delete %s. Reason: %s' % (file_path, e))
    if os.path.exists(directory_t):
        for filename in os.listdir(directory_t):
            file_path = os.path.join(directory_t, filename)
            try:
                if os.path.isfile(file_path) or os.path.islink(file_path):
                    os.unlink(file_path)
                elif os.path.isdir(file_path):
                    shutil.rmtree(file_path)
            except Exception as e:
                print('Failed to delete %s. Reason: %s' % (file_path, e))
                

# Divides the CSV files in Data/Lab2/Labeled into Train or Validation directory given the ratio. 
# Assumes all CSV files are valid files named correctly (ACT_###.csv).  

# The ratio is a ratio of train to validation. Defaults at 0.8:
# 80% of the data set will be classified for training, 20% for validation.

# Included optional checking procedure to ensure all activities are included in 
# both training and validation sets.   
def create_validation_set(ratio:float = 0.8, check:bool = True):
    clearFolder()
    directory = "../Data/Lab2/Labeled"
    validation_directory = "../Data/Lab2/Validation"
    train_directory = "../Data/Lab2/Train"
    validation_set = set()
    train_set = set()
    # set to check if all activities are included
    valid_set = {'STD', 'OHD', 'JOG', 'SIT', 'STR', 'TWS'}
    if os.path.exists(directory):
        for filename in os.listdir(directory):
            file_path = os.path.join(directory, filename)
            # RNG, determine training set
            if(random.random() <= ratio):
                shutil.copy2(file_path, train_directory)
                train_set.add(filename[:3])
            else:
                shutil.copy2(file_path, validation_directory)
                validation_set.add(filename[:3])

    # check whether all activities are included in both training and validation sets.
    if check:
        if validation_set == valid_set == train_set:
            print("At least one CSV file exists for each activity in both the training and validation sets.")
            print("You may proceed to the next step.")
        else:
            clearFolder()
            raise Exception("The given ratio failed to produce validation or train set with all six activities. Try changing the ratio or add more data sets. Files will be deleted.")


create_validation_set()