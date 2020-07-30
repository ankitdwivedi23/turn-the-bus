from pdf2image import convert_from_path, convert_from_bytes
from pathlib import Path
from PIL import Image
import tempfile
import os
import pytesseract
import time
from datetime import datetime
from flask import Flask, request, Response
from werkzeug.utils import secure_filename
from __init__ import webapp

PAGE_START = "PAGE_START"
PAGE_END = "PAGE_END"

uploads_dir = os.path.join(webapp.instance_path, 'uploads')
os.makedirs(uploads_dir, exist_ok=True)

@webapp.route('/pdf-to-text', methods=['POST'])
def pdf_to_text():    
    pdf_file = request.files.get('pdf')
    if pdf_file == None:
        return 'Missing pdf input in request body!'
    
    upload_path = os.path.join(uploads_dir, secure_filename(pdf_file.filename))
    pdf_file.save(upload_path)
    
    images = convert_from_path(upload_path)
    def image_to_text():
        all_text = ""     
        for img in images:
            text = pytesseract.image_to_string(img, lang='hin')
            yield text + "\n"    
    return Response(image_to_text())