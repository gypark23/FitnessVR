import data_analysis as DA
import pandas as pd

from tensorflow import keras
import numpy as np
import matplotlib.pyplot as plt
import glob
import os
from tensorflow.keras.models import Sequential
from tensorflow.keras.layers import LSTM, Dense
from sklearn.metrics import classification_report, confusion_matrix
import tensorflow as tf

from sklearn.linear_model import LinearRegression
from sklearn.linear_model import Lasso
from sklearn.ensemble import RandomForestRegressor
from sklearn.metrics import mean_squared_error
from sklearn.preprocessing import StandardScaler
from sklearn.pipeline import make_pipeline

from skforecast.ForecasterAutoreg import ForecasterAutoreg

# curl_df = DA.combine_samples("CUR")
# jump_df = DA.combine_samples("JUM")
path = "../../Data/FitnessVR/Train/"
valid_path = "../../Data/FitnessVR/Validation/"
valid_set = {'STD' : "CUR", 'SIT' : "JUM"}
def time_series_average(activity: str, sum: bool = False):
    # get all csv files
    
    csv_files = glob.glob(os.path.join(path, "*.csv"))
    df_list = []
    idx = 0

    for file in csv_files:
        name = file.split("\\")[-1].split('_')[0].rsplit('/', 1)[-1]
        # for the activity csv
        if name == activity:
            df_list.append(pd.read_csv(file))

    return pd.concat(df_list).groupby(level=0).sum() if sum else pd.concat(df_list).groupby(level=0).mean()
    
def concat_csv(dir):
    csv_files = glob.glob(os.path.join(dir, "*.csv"))
    df_list = []
    idx = 0

    for file in csv_files:
        name = file.split("\\")[-1].split('_')[0].rsplit('/', 1)[-1]
        # for the activity csv
        df = pd.read_csv(file)
        # 0 for Curl, 1 for Jump
        df['cat'] = 0 if (name == "CUR" or valid_set[name] == "CUR") else 1
        df_list.append(df)
    
    return pd.concat(df_list)

# Assume headset_pos.y is one of the significant features
jum = time_series_average(activity = "JUM")
jum_posy = jum["headset_pos.y"]
jum_posy = pd.Series(list(jum_posy), index=list(jum["time"]))
print(jum_posy)

jum_posy.plot(subplots=True)
plt.show()


# Prediction Model
from sklearn.model_selection import train_test_split

csvs = (concat_csv("../../Data/FitnessVR/Cleaned"))

X = csvs.drop("cat", axis=1).values
y = csvs["cat"].values

timestep = 1 #
num_samples = X.shape[0] // timestep
num_timesteps = timestep
num_features = X.shape[1]
X = X[:num_samples * timestep].reshape(num_samples, num_timesteps, num_features)
y = y[:num_samples * timestep].reshape(num_samples, num_timesteps)

X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.2, random_state=42)

# Use LSTM Deep Learning model for time-series
model = Sequential()
model.add(LSTM(64, input_shape=(num_timesteps, num_features)))
model.add(Dense(1, activation='sigmoid'))
model.compile(loss='binary_crossentropy', optimizer='adam', metrics=['accuracy'])

# Train the model
model.fit(X_train, y_train, epochs=10, batch_size=32, validation_data=(X_test, y_test))

loss, accuracy = model.evaluate(X_test, y_test)
print('Test accuracy:', accuracy)
print('Test loss:', loss)

# Prediction
y_pred = model.predict(X_test)
y_pred = np.round(y_pred)

# Evaluation
print(classification_report(y_test.ravel(), y_pred.ravel()))
print(confusion_matrix(y_test.ravel(), y_pred.ravel()))

# # Compare predicted class and actual class
# # Note 0 = Curl, 1 = Jump
# for i in range(len(y_pred)):
#     print('Actual:', y_test[i], 'Predicted:', y_pred[i])

# Save model
# tf.saved_model.save(model, "saved_model")





# Time-Series Evaluation for Posture
# Only use headset_pos.y

# Data Cleaning 
steps = 159
data = pd.read_csv("../../Data/FitnessVR/Train/JUM_P1_02.csv")
train, test = data.iloc[:560],  data.iloc[560:]
data = data.set_index('time')
# fig, ax=plt.subplots(figsize=(9, 4))
# train.plot(ax = ax, y = 'headset_pos.y', label='train')
# test.plot(ax = ax, y = 'headset_pos.y', label='test')

# ax.legend()
# plt.show()

# Predict
forecaster = ForecasterAutoreg(regressor = RandomForestRegressor(),lags= 100)
forecaster.fit(y=train['headset_pos.y'])
predictions = forecaster.predict(steps=steps)

# Visualize Prediction
fig, ax = plt.subplots(figsize=(9, 4))
train.plot(ax=ax, y = "headset_pos.y", label='train')
test.plot(ax=ax, y = "headset_pos.y", label='test')
predictions.plot(ax=ax, x = test, label='predictions')
ax.legend()
plt.savefig("../../Reports/FitnessVR/time_series_forecast.png")
plt.show()

# Evaluation
error_mse = mean_squared_error(y_true = test["headset_pos.y"],y_pred = predictions)
print(f"Test error (mse): {error_mse}")



