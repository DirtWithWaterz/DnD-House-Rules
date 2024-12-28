/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextWriter : MonoBehaviour {

    private static TextWriter instance;

    private List<TextWriterSingle> textWriterSingleList;

    private void Awake() {
        instance = this;
        textWriterSingleList = new List<TextWriterSingle>();
    }

    public static TextWriterSingle AddWriter_Static(TMP_Text uiText, string textToWrite, float timePerCharacter, bool invisibleCharacters, bool removeWriterBeforeAdd, Action onComplete) {
        if (removeWriterBeforeAdd) {
            instance.RemoveWriter(uiText);
        }
        return instance.AddWriter(uiText, textToWrite, timePerCharacter, invisibleCharacters, onComplete);
    }

    private TextWriterSingle AddWriter(TMP_Text uiText, string textToWrite, float timePerCharacter, bool invisibleCharacters, Action onComplete) {
        TextWriterSingle textWriterSingle = new TextWriterSingle(uiText, textToWrite, timePerCharacter, invisibleCharacters, onComplete);
        textWriterSingleList.Add(textWriterSingle);
        return textWriterSingle;
    }

    public static void RemoveWriter_Static(TMP_Text uiText) {
        instance.RemoveWriter(uiText);
    }

    private void RemoveWriter(TMP_Text uiText) {
        for (int i = 0; i < textWriterSingleList.Count; i++) {
            if (textWriterSingleList[i].GetUIText() == uiText) {
                textWriterSingleList.RemoveAt(i);
                i--;
            }
        }
    }

    private void Update() {
        for (int i = 0; i < textWriterSingleList.Count; i++) {
            bool destroyInstance = textWriterSingleList[i].Update();
            if (destroyInstance) {
                textWriterSingleList.RemoveAt(i);
                i--;
            }
        }
    }

    /*
     * Represents a single TextWriter instance
     * */
    public class TextWriterSingle {

        private TMP_Text uiText;
        private string textToWrite;
        private int characterIndex;
        private float timePerCharacter;
        private float timer;
        private bool invisibleCharacters;
        private Action onComplete;

        public TextWriterSingle(TMP_Text uiText, string textToWrite, float timePerCharacter, bool invisibleCharacters, Action onComplete) {
            this.uiText = uiText;
            this.textToWrite = textToWrite;
            this.timePerCharacter = timePerCharacter;
            this.invisibleCharacters = invisibleCharacters;
            this.onComplete = onComplete;
            characterIndex = 0;
        }

        // Returns true on complete
        public bool Update() {
            if(textToWrite[textToWrite.Substring(0, characterIndex).Length].ToString() == "."){
                timer -= Time.deltaTime / 10f;
            }
            else if(textToWrite[textToWrite.Substring(0, characterIndex).Length].ToString() == ","){
                timer -= Time.deltaTime / 8f;
            }
            else if(textToWrite[textToWrite.Substring(0, characterIndex).Length].ToString() == "q"){
                timer -= Time.deltaTime / 2f;
            }
            else if(textToWrite[textToWrite.Substring(0, characterIndex).Length].ToString() == "w"){
                timer -= Time.deltaTime / 1f;
            }
            else if(textToWrite[textToWrite.Substring(0, characterIndex).Length].ToString() == "e"){
                timer -= Time.deltaTime / 1f;
            }
            else if(textToWrite[textToWrite.Substring(0, characterIndex).Length].ToString() == "r"){
                timer -= Time.deltaTime / 1f;
            }
            else if(textToWrite[textToWrite.Substring(0, characterIndex).Length].ToString() == "t"){
                timer -= Time.deltaTime / 2f;
            }
            else if(textToWrite[textToWrite.Substring(0, characterIndex).Length].ToString() == "y"){
                timer -= Time.deltaTime / 2f;
            }
            else if(textToWrite[textToWrite.Substring(0, characterIndex).Length].ToString() == "u"){
                timer -= Time.deltaTime / 2f;
            }
            else if(textToWrite[textToWrite.Substring(0, characterIndex).Length].ToString() == "i"){
                timer -= Time.deltaTime / 1f;
            }
            else if(textToWrite[textToWrite.Substring(0, characterIndex).Length].ToString() == "o"){
                timer -= Time.deltaTime / 0.8f;
            }
            else if(textToWrite[textToWrite.Substring(0, characterIndex).Length].ToString() == "p"){
                timer -= Time.deltaTime / 1f;
            }
            else if(textToWrite[textToWrite.Substring(0, characterIndex).Length].ToString() == "a"){
                timer -= Time.deltaTime / 0.4f;
            }
            else if(textToWrite[textToWrite.Substring(0, characterIndex).Length].ToString() == "s"){
                timer -= Time.deltaTime / 0.5f;
            }
            else if(textToWrite[textToWrite.Substring(0, characterIndex).Length].ToString() == "d"){
                timer -= Time.deltaTime / 0.4f;
            }
            else if(textToWrite[textToWrite.Substring(0, characterIndex).Length].ToString() == "f"){
                timer -= Time.deltaTime / 0.8f;
            }
            else if(textToWrite[textToWrite.Substring(0, characterIndex).Length].ToString() == "g"){
                timer -= Time.deltaTime / 1f;
            }
            else if(textToWrite[textToWrite.Substring(0, characterIndex).Length].ToString() == "h"){
                timer -= Time.deltaTime / 0.6f;
            }
            else if(textToWrite[textToWrite.Substring(0, characterIndex).Length].ToString() == "j"){
                timer -= Time.deltaTime / 0.6f;
            }
            else if(textToWrite[textToWrite.Substring(0, characterIndex).Length].ToString() == "k"){
                timer -= Time.deltaTime / 2f;
            }
            else if(textToWrite[textToWrite.Substring(0, characterIndex).Length].ToString() == "l"){
                timer -= Time.deltaTime / 2f;
            }
            else if(textToWrite[textToWrite.Substring(0, characterIndex).Length].ToString() == "z"){
                timer -= Time.deltaTime / 2f;
            }
            else if(textToWrite[textToWrite.Substring(0, characterIndex).Length].ToString() == "x"){
                timer -= Time.deltaTime / 2f;
            }
            else if(textToWrite[textToWrite.Substring(0, characterIndex).Length].ToString() == "c"){
                timer -= Time.deltaTime / 1f;
            }
            else if(textToWrite[textToWrite.Substring(0, characterIndex).Length].ToString() == "v"){
                timer -= Time.deltaTime / 1f;
            }
            else if(textToWrite[textToWrite.Substring(0, characterIndex).Length].ToString() == "b"){
                timer -= Time.deltaTime / 2f;
            }
            else if(textToWrite[textToWrite.Substring(0, characterIndex).Length].ToString() == "n"){
                timer -= Time.deltaTime / 1f;
            }
            else if(textToWrite[textToWrite.Substring(0, characterIndex).Length].ToString() == "m"){
                timer -= Time.deltaTime / 2f;
            }
            else{
                timer -= Time.deltaTime;
            }

            while (timer <= 0f) {
                // Display next character
                timer += timePerCharacter;
                characterIndex++;
                string text = textToWrite.Substring(0, characterIndex);
                if (invisibleCharacters) {
                    text += "<color=#00000000>" + textToWrite.Substring(characterIndex) + "</color>";
                }
                uiText.text = text;

                if (characterIndex >= textToWrite.Length) {
                    // Entire string displayed
                    if (onComplete != null) onComplete();
                    return true;
                }
            }

            return false;
        }

        public TMP_Text GetUIText() {
            return uiText;
        }

        public bool IsActive() {
            return characterIndex < textToWrite.Length;
        }

        public void WriteAllAndDestroy() {
            uiText.text = textToWrite;
            characterIndex = textToWrite.Length;
            if (onComplete != null) onComplete();
            TextWriter.RemoveWriter_Static(uiText);
        }


    }


}
