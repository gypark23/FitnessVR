import argparse
from glob import glob

"""
Examine the mean and variance of each activity’s sensor data, and build a statistical 
threshold-based classifier for activity detection.

Usage:
    
    python3 Python/predict_stat_thresh.py <sensor .csv sample>

    python3 Python/predict_stat_thresh.py --label_folder <folder with sensor .csv samples>
"""


def predict_stat_thresh(sensor_data_path: str) -> str:
    """Run prediction on a sensor data sample.

    Replace the return value of this function with the output activity label
    of your stat-based threshold model. Feel free to load any files and write
    helper functions as needed.
    """
    return "JOG"


def predict_stat_thresh_folder(data_folder: str, output_file: str):
    """Run the model's prediction on all the sensor data in data_folder, writing labels
    in sequence to an output text file."""

    data_files = sorted(glob(f"{data_folder}/*.csv"))
    labels = map(predict_stat_thresh, data_files)

    with open(output_file, "w+") as output_file:
        output_file.write("\n".join(labels))


if __name__ == "__main__":
    # Parse arguments to determine whether to predict on a file or a folder
    # You should not need to modify the below starter code, but feel free to
    # add more arguments for debug functions as needed.
    parser = argparse.ArgumentParser()

    sample_input = parser.add_mutually_exclusive_group(required=True)
    sample_input.add_argument(
        "sample", nargs="?", help="A .csv sensor data file to run predictions on"
    )
    sample_input.add_argument(
        "--label_folder",
        type=str,
        required=False,
        help="Folder of .csv data files to run predictions on",
    )

    parser.add_argument(
        "--output",
        type=str,
        default="Data/Lab2/Labels/stat.txt",
        help="Output filename of labels when running predictions on a directory",
    )

    args = parser.parse_args()

    if args.sample:
        print(predict_stat_thresh(args.sample))

    elif args.label_folder:
        predict_stat_thresh_folder(args.label_folder, args.output)