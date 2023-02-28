import data_analysis as DA
import pandas as pd



curl_df = DA.combine_samples("CUR")
jump_df = DA.combine_samples("JUM")

print(curl_df)