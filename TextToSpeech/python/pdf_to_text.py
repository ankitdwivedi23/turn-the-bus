from pdf2image import convert_from_path, convert_from_bytes
from pathlib import Path
from PIL import Image
import tempfile
import os
import pytesseract
import time
from datetime import datetime
from flask import Flask, request
from werkzeug.utils import secure_filename
from __main__ import app

PAGE_START = "PAGE_START"
PAGE_END = "PAGE_END"

uploads_dir = os.path.join(app.instance_path, 'uploads')
os.makedirs(uploads_dir, exist_ok=True)

@app.route('/pdf-to-text', methods=['POST'])
def pdf_to_text():
    all_text = ""
    pdf_file = request.files.get('pdf')
    if pdf_file == None:
        return 'Missing pdf input in request body!'
    
    upload_path = os.path.join(uploads_dir, secure_filename(pdf_file.filename))
    pdf_file.save(upload_path)

    images = convert_from_path(upload_path)
    for img in images:
        text = pytesseract.image_to_string(img, lang='hin')
        all_text = all_text + "\n" + text + "\n"
    return all_text