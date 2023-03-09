import IK_solver as IK
import visualize_arm as VA
from preprocess import load_poses
import numpy as np


# solve for joint angles for the right arm
def get_right_angles(dir, sample_rate=10, show=False):
	_, r_poses = load_poses(dir, sample_rate)

	angles = []
	for pose in r_poses:
		pose = IK.transform_coordinates(pose)
		angle = IK.inverse_kinematics_solver_Jacobian(100000, pose)
		if angle != None:
			angles.append(angle)
			if show:
				print("    Angle:", angle)

	return angles

# evaluate total squared diff between two groups of exercises
def rate(standard_angles, angles):
	time = len(standard_angles)
	standard_angles = [np.array(angle) for angle in standard_angles]
	angles = [np.array(angle) for angle in angles][0:time]

	diff = 0 
	for i in range(time):
		diff += sum((standard_angles[i] - angles[i])**2)

	print("Rating for the exercise:", 1/diff*100000)
	return diff


# ============================== MAIN =================================
dir_1 = "../../../Data/FitnessVR/Cleaned/SIT_P1_02.csv"
dir_2 = "../../../Data/FitnessVR/Cleaned/SIT_P1_04.csv"

ang1 = get_right_angles(dir_1)
ang2 = get_right_angles(dir_2)
rate(ang1, ang2)