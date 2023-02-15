from flask import Flask, flash, request, redirect, url_for
from flask_cors import CORS, cross_origin
from model import predict
from datetime import datetime
import numpy as np
import cv2 as cv
import os

# Khởi tạo Flask Server Backend
app = Flask(__name__)

# Apply Flask CORS
CORS(app)
app.config['CORS_HEADERS'] = 'Content-Type'
app.config['UPLOAD_FOLDER'] = ''

@app.route('/predict', methods=['POST', 'GET'])
@cross_origin(origin='*')
def home():
    if request.method == 'POST':
        image = request.files['file'].read()
        file_bytes = np.fromstring(image, np.uint8)
        img = cv.imdecode(file_bytes, cv.IMREAD_UNCHANGED)
        imgRGB = cv.cvtColor(img, cv.COLOR_BGR2RGB)
        res = predict(imgRGB)

        path = "images\\" + res
        # Check whether the specified path exists or not
        isExist = os.path.exists(path)
        if not isExist:
        # Create a new directory because it does not exist
            os.makedirs(path)

        now = datetime.now()
        dt_string = now.strftime("%d-%m-%Y-%H-%M-")

        if len(res) >= 8:
            img_name = dt_string+res+".png"
            cv.imwrite("images\\" + res + "\\" + img_name, img)
            return [res, img_name]
        else:
            return 'Detect license plate false! Please try again'
    if request.method == 'GET':
        return 'test'
    return 'no method detect'

# Start Backend
if __name__ == '__main__':
    app.run(host='0.0.0.0', port=5000)
