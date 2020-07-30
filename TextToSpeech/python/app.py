from __init__ import webapp as app

from pdf_to_text import pdf_to_text

@app.route('/', methods=['GET'])
def default():
    return 'Welcome to the Text-To-Speech API for Turn The Bus!'

if __name__ == "__main__":
    app.run(debug=True)