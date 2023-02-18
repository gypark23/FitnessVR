import pandas as pd
import numpy as np
from sklearn.tree import DecisionTreeRegressor
from sklearn.model_selection import train_test_split
from sklearn.metrics import mean_squared_error
from skl2onnx import convert_sklearn
from skl2onnx.common.data_types import FloatTensorType
import onnx

#Example Decision Tree Model
x = list(range(1, 21))
y = [3*i for i in x]

data = {'x': x, 'y': y}
df = pd.DataFrame(data)

X_train, X_test, y_train, y_test = train_test_split(df[['x']], df['y'], test_size=0.2)

model = DecisionTreeRegressor()
model.fit(X_train, y_train)

y_pred = model.predict(X_test)


mse = mean_squared_error(y_test, y_pred)
print('Mean squared error:', mse)

results = pd.DataFrame({'x': X_test['x'], 'Actual Value': y_test, 'Predicted Value': y_pred})
print(results)


#import to ONNX model
initial_type = [('float_input', FloatTensorType([1, 4]))]
onnx_model = convert_sklearn(model, initial_types=initial_type)
onnx.save_model(onnx_model, '../../Assets/test.onnx')