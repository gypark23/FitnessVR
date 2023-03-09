import math
from math import sin, cos
import numpy as np

# ============================== CONFIG OF HUMAN ARM =================================
b = 0
l1 = 0.2 	# human central position to shoulder
l2 = 0.3 	# length of the upper arm
l3 = 0.3 	# length of the lower arm


# ============================== IK SOLVER =================================
def transform_coordinates(pose, inv=False):
	[x, y, z, alpha, beta, gamma] = pose
	# from model coordinates to robot coordinates
	if inv:
		return [y, -x, z, alpha, beta, gamma]
	# from robot coordinates to model coordiates
	else:
		return [-y, x, z, alpha, beta, gamma]


def find_Jacobian(curr_angle):
	[q1, q2, q3, q4] = curr_angle

	j00 = l2*cos(q1)*sin(q2) - l3*(cos(q1)*cos(q2)*cos(q3) - cos(q1)*sin(q2)*sin(q3))
	j01 = l3*(cos(q2)*sin(q1)*sin(q3) + cos(q3)*sin(q1)*sin(q2)) + l2*cos(q2)*sin(q1)
	j02 = l3*(cos(q2)*sin(q1)*sin(q3) + cos(q3)*sin(q1)*sin(q2))

	j10 = l3*(sin(q1)*sin(q2)*sin(q3) - cos(q2)*cos(q3)*sin(q1)) + l2*sin(q1)*sin(q2)
	j11 = -l3*(cos(q1)*cos(q2)*sin(q3) + cos(q1)*cos(q3)*sin(q2)) - l2*cos(q1)*cos(q2)
	j12 = -l3*(cos(q1)*cos(q2)*sin(q3) + cos(q1)*cos(q3)*sin(q2))

	j20 = 0
	j21 = l3*(cos(q2)*cos(q3) - sin(q2)*sin(q3)) - l2*sin(q2)
	j22 = l3*(cos(q2)*cos(q3) - sin(q2)*sin(q3))

	j0 = [j00, j01, j02]
	j1 = [j10, j11, j12]
	j2 = [j20, j21, j22]
	J = np.matrix([j0, j1, j2])
	return J


def foward_kinematics_solver_Jacobian(curr_angle):
	[q1, q2, q3, q4] = curr_angle
	x = l2*sin(q1)*sin(q2) + l3*(cos(q2)*cos(q3)*sin(q1) - sin(q1)*sin(q2)*sin(q3))
	y = -l2*cos(q1)*sin(q2) + l3*(cos(q2)*cos(q3)*cos(q1) - cos(q1)*sin(q2)*sin(q3))
	z = b + l1 + l2*cos(q2) + l3*(cos(q2)*sin(q3) + cos(q3)*sin(q2))
	return [x, y, z]


def inverse_kinematics_solver_Jacobian(max_trial, target_pose, show=False):
	
	# initialization 
	curr_angle = [0,0,0,0]
	curr_pose = [0,0,0,0,0,0]
	[x_t, y_t, z_t, alpha_t, beta_t, gamma_t] = target_pose

	count = 0
	prev_pose_abs_diff = 1
	
    
	while(1):
		[q1, q2, q3, q4] = curr_angle
		[x, y, z, alpha, beta, gamma] = curr_pose

		# find the difference to target pose
		pose_abs_diff = (x_t-x)**2 + (y_t-y)**2 + (z_t-z)**2
		pose_diff = 0.01 * np.matrix([x_t-x, y_t-y, z_t-z]).transpose()

		# stay in loop if the difference between current and target pose keeps decreasing
		if (pose_abs_diff > prev_pose_abs_diff):
			break
		else:
			prev_pose_abs_diff = pose_abs_diff

		# compute pseudo inverse of J based on current joint angles
		J = find_Jacobian(curr_angle)
		J_inv = np.linalg.pinv(J)

		# calculate intermediate angles and pose
		ang_diff = np.matmul(J_inv, pose_diff)
		q1 -= ang_diff[0,0]
		q2 += ang_diff[1,0]
		q3 += ang_diff[2,0]

		# update intermediate angles and pose
		curr_angle = [q1, q2, q3, gamma_t]
		curr_pose = foward_kinematics_solver_Jacobian(curr_angle) + [0,0,gamma_t]
		count += 1

		# break if can't find solution in max number of trials
		if (count == max_trial):
			print("    Solution not Found!")
			return None

	if show:
		print("Solution found after "+ str(count) + " iterations")
		print("Angles: " + str(curr_angle))

	return curr_angle




# ============================== MAIN =================================
# pose = foward_kinematics_solver_Jacobian([0.1, 0.15, 0.5, 0]) + [0,0,0.15]
# ang = inverse_kinematics_solver_Jacobian(100000, pose, show=True)