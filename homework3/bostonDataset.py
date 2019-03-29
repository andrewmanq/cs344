import pandas as pd

# unfortunately I still could not get keras to work correctly. Instead, I've used a
# similar library called sklearn, which imports the exact same boston housing data.
from sklearn.datasets import load_boston

import numpy as np
boston_dataset = load_boston()
pd.__version__

# -----------------------------here's what the columns mean--------------------------------
# CRIM: Per capita crime rate by town
# ZN: Proportion of residential land zoned for lots over 25,000 sq. ft
# INDUS: Proportion of non-retail business acres per town
# CHAS: Charles River dummy variable (= 1 if tract bounds river; 0 otherwise)
# NOX: Nitric oxide concentration (parts per 10 million)
# RM: Average number of rooms per dwelling
# AGE: Proportion of owner-occupied units built prior to 1940
# DIS: Weighted distances to five Boston employment centers
# RAD: Index of accessibility to radial highways
# TAX: Full-value property tax rate per $10,000
# PTRATIO: Pupil-teacher ratio by town
# B: 1000(Bk — 0.63)², where Bk is the proportion of [people of African American descent] by town
# LSTAT: Percentage of lower status of the population
# MEDV: Median value of owner-occupied homes in $1000s

# loading the whole dataset into a dataframe
boston_housing_dataframe = pd.DataFrame(boston_dataset.data, columns=boston_dataset.feature_names)

# shuffles data
boston_housing_dataframe = boston_housing_dataframe.reindex(
    np.random.permutation(boston_housing_dataframe.index))

# outputting some info about the data
print("dimensions and other info about these training sets:")
print(boston_housing_dataframe.describe())
print("rows and columns of full dataset: " + str(boston_housing_dataframe.shape))

# training data - Since the whole table is 506 rows, I divided it into 3 parts:
# 304 items for training, 100 items for testing, 100 items for validation.
training = boston_housing_dataframe[0:304]
print("rows and columns of training data: " + str(training.shape))

# testing data - 100 items
testing = boston_housing_dataframe[305:405]
print("rows and columns of testing data: " + str(testing.shape))

# validation data - 100 items
validation = boston_housing_dataframe[406:506]
print("rows and columns of validation data: " + str(validation.shape))


# for a synthetic feature, I'm receiving the product of crime rate (CRIM) and pupil-teacher ratio(PTRATIO).
# this could be a valuable gauge of factors that affect both education quality and criminal activity.
# since a parent would want their child to grow up in a crime-free area with plenty of teachers, I'll
# call this feature child_unfriendliness.

boston_housing_dataframe['child_unfriendliness'] = boston_housing_dataframe['CRIM'] * boston_housing_dataframe['PTRATIO']
print(boston_housing_dataframe.describe())
