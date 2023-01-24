import pandas as pd
import os
import glob
import matplotlib.pyplot as plt
import numpy as np
import data_analysis as DA


dir = "../Data/Lab1/"


class Activity:
	def __init__(self, activity_name):
		self.name = activity_name
		self.feature_df = DA.combine_samples(activity_name)

		# average of mean and variance of an activity from the training set
		self.mean_array = self.feature_df['Mean'].to_numpy()
		self.var_array = self.feature_df['Variance'].to_numpy()

		# initial overall weights for the 2 categories of data: mean and var
		self.mean_superweight = 1
		self.var_superweight = 1

		# initial weights for specific features
		self.mean_weights = np.array(36*[1])
		self.var_weights = np.array(36*[1])

	def update_weights(self, new_weights):
		try:
			self.mean_superweight =  new_weights[0]
			self.mean_weights = np.array(new_weights[1])
			self.var_superweight =  new_weights[2]
			self.var_weights = np.array(new_weights[3])
		except:
			print("Error: wrong dimension!")

	def print_weights(self):
		print("Mean weights: ", self.mean_superweight*self.mean_weights)
		print("Var weights:  ", self.var_superweight*self.var_weights)

	def compare(self, activity_df):
		mean_array = activity_df['Mean'].to_numpy()
		var_array = activity_df['Variance'].to_numpy()
		
		mean_dist = ((mean_array - self.mean_array)/self.mean_array)**2
		var_dist = ((var_array - self.var_array)/self.var_array)**2

		dist = 0
		dist += self.mean_superweight * (mean_dist * self.mean_weights).sum()
		dist += self.var_superweight * (var_dist * self.var_weights).sum()
		return dist



class Classifier:
	def __init__(self, activities):
		self.activities = []
		self.names = []
		for activitiy in activities:
			self.activities.append(activitiy)
			self.names.append(activitiy.name)

	def summarize(self):
		print("Activities: ", self.names)

	def update_weights(self, new_weights):
		for activity in self.activities:
			activity.update_weights(new_weights)

	def print_weights(self):
		try:
			self.activities[0].print_weights()
		except:
			print("Error: no activity loaded!")

	def classify(self, activity_df):
		dists = []
		for activitiy in self.activities:
			dists.append(activitiy.compare(activity_df))

		dists = np.array(dists)
		category = self.names[np.argmin(dists)]
		dists = np.sort(dists)

		return category, dists


def test(classifier, tasks, test_name, show_accuracy = True):
	print("")
	print(test_name)

	hit = 0
	for task in tasks:
		task_df = DA.summarize_sensor_trace(dir + task)
		category, dists = classifier.classify(task_df)

		if task[0:3] == category:
			hit += 1

		print("    Activity ", task, " is classified as: ", category, end="  ")
		print("    Dists = ", [int(dist) for dist in dists])
	
	if show_accuracy:
		accuracy = hit / len(tasks)
		print("\n    Accuracy: ", accuracy, "\n")




# =================================================== testing ==================================================


# building a list of activities
Standing = Activity("STD")
Sitting = Activity("SIT")
Jogging = Activity("JOG")
Stretching = Activity("STR")
Overhead = Activity("OHD")
Twisting = Activity("TWS")
activities = [Standing, Sitting, Jogging, Stretching, Overhead, Twisting]

# build a classifier with given activities and weights
mean_superweight =  1
mean_weights = 36*[1]
var_superweight =  1
var_weights = 36*[1]

# raise the importance of head pos mean
mean_weights[6] = 3
mean_weights[7] = 8
mean_weights[8] = 3

# raise the importance of head pos var
var_weights[6] = 3

# raise the importance of controller rot, pos var
var_weights[21] = 3
var_weights[33] = 3

# build list of weights
new_weights = [mean_superweight, mean_weights, var_superweight, var_weights]

C = Classifier(activities)
C.update_weights(new_weights)
C.print_weights()



# testing 1
tasks = ["JOG_P2_04.csv", "STD_P3_05.csv", "STR_P2_05.csv", "OHD_P1_04.csv", "TWS_P3_04.csv", "SIT_P1_04.csv"]
test(C, tasks, "Test 1")

# testing 2
tasks = ["JOG_P1_04.csv", "STD_P1_01.csv", "STR_P3_05.csv", "OHD_P3_04.csv", "TWS_P2_04.csv", "SIT_P1_02.csv"]
test(C, tasks, "Test 2")
