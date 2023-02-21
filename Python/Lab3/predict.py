import os
import time
import warnings
warnings.filterwarnings("ignore")

import pandas as pd
from sklearn.ensemble import RandomForestClassifier
from sklearn.model_selection import train_test_split
from preprocess import create_dataset


start_time = time.time()

# Train the model on the entire dataset
training_set, labels = create_dataset(dir_1)
rf.fit(training_set, labels)
train_time = time.time()

# Testing
testing_set, new_labels = create_dataset(dir_2)
pred_labels = rf.predict(testing_set)
accuracy = accuracy_score(new_labels, pred_labels)
end_time = time.time()


print('Random Forest accuracy:', accuracy)
print(f"Training time: {train_time - start_time} seconds")
print(f"Testing time: {end_time - train_time} seconds")














