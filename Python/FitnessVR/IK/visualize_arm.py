from mpl_toolkits.mplot3d import Axes3D
import matplotlib.pyplot as plt
import numpy as np
from IK_solver import foward_kinematics_solver_Jacobian




# ============================== CONFIG OF HUMAN ARM =================================
l1 = 0.3 # length of upper arm
l2 = 0.3 # length of lower arm


# ============================== 3D PLOT FUNCTIONS =================================
# plot the arm in 3d coordinates
def plot_pose(angle, ax):
	_, theta1, theta2, _, = angle
	# Define the positions of the joints
	joint1_pos = np.array([0, 0, 0])
	joint2_pos = joint1_pos + np.array([0, l1*np.cos(theta1), l1*np.sin(theta1)])
	joint3_pos = foward_kinematics_solver_Jacobian(angle)
	joints_pos = np.array([joint1_pos, joint2_pos, joint3_pos])

	# Plot the arm
	ax.plot(joints_pos[:,0], joints_pos[:,1], joints_pos[:,2], 'bo-', linewidth=2, markersize=12)
	ax.plot(joint1_pos[0], joint1_pos[1], joint1_pos[2], 'ro', markersize=12)

	ax.set_xlim([-0.5, 0.5])
	ax.set_ylim([-0.5, 0.5])
	ax.set_zlim([-0.5, 0.5])
	return

# plot the pose of the entire series
def plot_pose_series(angles, num_cols=3):
	# setting the grid
    num_plots = len(angles)
    num_rows = num_plots // num_cols + 1

    fig = plt.figure(figsize=(10, 3*num_rows))

    for i, angle in enumerate(angles):
        row = i // num_cols
        col = i % num_cols
        ax = fig.add_subplot(num_rows, num_cols, i+1, projection='3d')
        plot_pose(angle, ax)
        ax.set_title("t=" + str(i))
        
    plt.tight_layout()
    plt.show()

