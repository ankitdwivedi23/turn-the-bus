# -*- coding: UTF-8 -*-

def create_ssml(s, filename):
    
    words = s.split(' ')
    part_file = []
    cnt = 0
    j = 0
    i = 0
    while i < len(words):
        if cnt + len(words[i]) + 2 < 5000:
            cnt += len(words[i]) + 2
            part_file.append(words[i])
            i += 1
        else:
            
            sentences = []
            k = 0
            sentence = []
            while k < len(part_file):
                if part_file[k].strip() != '।' and part_file[k].strip() != '!':
                    
                    para_split = part_file[k].split('\n\n')
                    clean_words = []
                    for p, elem in enumerate(para_split):
                        word = ' '.join(para_split[p].split('\n')).strip()
                        clean_words.append(word)
                    sentence.append('\n\n'.join(clean_words))
                else:
                    sentence.append(part_file[k])
                    sentences.append(' '.join(sentence))
                    sentence = []
                k+=1
            
            if (len(sentence) > 0):
                sentences.append(' '.join(sentence))
            
            text = ' '.join(sentences)
            final = '<speak version="1.0" xmlns="https://www.w3.org/2001/10/synthesis" xml:lang="en-US">  <voice name="hi-IN-Kalpana">' + text + '  </voice></speak>'
            output_f = open(f'Digant/{filename}_part{j+1}.xml', 'w', encoding='utf-8')
            output_f.write(final)
            output_f.close()
            j += 1
            cnt = 0
            part_file = []
    
    text = ' '.join(part_file)
    final = '<speak version="1.0" xmlns="https://www.w3.org/2001/10/synthesis" xml:lang="en-US">  <voice name="hi-IN-Kalpana">' + text + '  </voice></speak>'
    output_f = open(f'Digant/{filename}_part{j+1}.xml', 'w', encoding='utf-8')
    output_f.write(final)
    output_f.close()
        

if __name__ == "__main__":
    list_chapters = ["बातचीत", "उसने कहा था .","संपूर्ण क्रांति","अर्धनारीश्वर","रोज","आओ सदानीरा","सिपाही की माँ", "“प्रगीत ' और समाज","जूठन","हँसते हुए मेरा अकेलापन","तिरिछ","शिक्षा","कड़बक","पद","छप्पय","कवित्त","तुमुल कोलाहल कलह में","पुत्र वियोग","जन-जन का चेहरा एक","प्यारे ननहें बेटे को","हार-जीत","गाँव का घर"]

    list_writers = ["बालकृष्ण भट्ट","चंद्रधर शर्मा गुलेरी","जयप्रकाश नारायण","रामधारी सिंह दिनकर","सच्चचिदानंद हीरानंद वात्स्यायन अज्ञेय","जगदीशचंद्र माथुर","मोहन राकेश","नामवर सिंह","ओमप्रकाश वाल्मीकि","मलयज","उदय प्रकाश","जे० कृष्णमूर्ति","मलिक मुहम्मद जायसी","तुलसीदास","नाभादास","भूषण","जयशंकर प्रसाद","सुभद्रा कुमारी चौहान","गजानन माधव मुक्तिबोध","विनोद कुमार शुक्ल","अशोक वाजपेयी","ज्ञानेंद्रपति"]

    input_f = open('Digant/Digant.txt', 'r', encoding='utf-8') # Read input file
    input_lines = input_f.readlines()
    input_f.close()

    i = 0
    j = 0 
    while input_lines[i].strip() != 'रचनाएँ':
        i+=1
    while i < len(input_lines) and j < len(list_writers):
        str1 = ""
        str2 = ""
        if(input_lines[i].strip() == list_writers[j]): # match writer name
            while(input_lines[i].strip() != list_chapters[j]): # save content till chapter name appears
                str1 = str1 + input_lines[i]
                i+=1
            create_ssml(str1, f'chapter{j+1}_biography')
            while(input_lines[i].strip() != "अभ्यास"  and input_lines[i].strip() != "अभ्यास ." ): # save content till abhyas word
                str2 = str2 + input_lines[i]
                i+=1
            create_ssml(str2, f'chapter{j+1}_content')
            j+=1
        i+=1
