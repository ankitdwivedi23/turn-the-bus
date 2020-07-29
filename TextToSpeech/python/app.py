from flask import Flask, request

app = Flask(__name__) #create the Flask app

from pdf_to_text import pdf_to_text

@app.route('/', methods=['GET'])
def default():
    return 'Welcome to the Text-To-Speech API for Turn The Bus!'

if __name__ == "__main__":
    app.run()