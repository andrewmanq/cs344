"""
spamFilter.py

provides a class and test case for spam recognition by using statistical probability models.

author:Andrew Quist
"""

class spamFilterProbability:
    """
    input:  spamCorpus is a list of word vectors that contain spam-related words
            legitCorpus is a list of word vectors that contain non-spam words
    """
    def __init__(self, spamCorpus, legitCorpus):
        size = len(spamCorpus)
        """
        spamdict is the dictionary of spam words, and nbad is the number of spam messages
        """
        self.spamDict = {}
        self.nbad = 0
        for i in range(0, size):
            self.nbad += 1
            for j in range(0, len(spamCorpus[i])):
                word = spamCorpus[i][j].lower()
                if word in self.spamDict.keys():
                    self.spamDict[word] += 1
                else:
                    self.spamDict[word] = 1
        """
        legitDict is the dictionary of non-spam words, and ngood is the number of non-spam messages
        """
        self.legitDict = {}
        self.ngood = 0
        for i in range(0, size):
            self.ngood += 1
            for j in range(0, len(legitCorpus[i])):
                word = legitCorpus[i][j].lower()
                if word in self.legitDict.keys():
                    self.legitDict[word] += 1
                else:
                    self.legitDict[word] = 1

    """
    A simple function that returns the good/bad distribution in the order [num spam words, num non-spam words]
    """
    def getWordFrequency(self, word):
        word = word.lower()
        b = 0
        if word in self.spamDict:
            b = self.spamDict[word]

        g = 0
        if word in self.legitDict:
            g = self.legitDict[word]

        return [b, g]

    """
    Calculates the probability that the word is from a spam email. if not found in either realm,
    the function returns .05
    """
    def getWordProbability(self, word):
        comparison = self.getWordFrequency(word)
        b = comparison[0]
        g = comparison[1] * 2

        if (b + g) != 0:
            answer = max(0.01, min(0.99, min(1.0, b/self.nbad) / (min(1.0, g/self.ngood) + min(1.0, b/self.nbad))))

            if answer >= 1:
                answer = .99
            elif answer <= 0:
                answer = .01

            return answer
        else:
            return 0.5
    """
    returns a list of [word, probability] for all words in the full text
    """
    def getProbabilityList(self, fullText):
        allWordProbabilities = {}
        for i in range(0, len(fullText)):
            newWord = fullText[i]
            if newWord not in allWordProbabilities:
                allWordProbabilities[newWord] = self.getWordProbability(newWord)
        answerList = []
        for key, value in allWordProbabilities.items():
            temp = [key, value]
            answerList.append(temp)
        return answerList

    "returns the combined probability of all words, gives an estimate on how likely that the message is spam."
    def getCombinedProbability(self, fullText):
        probList = self.getProbabilityList(fullText)
        product = 0
        invProduct = 0
        for word in probList:
            if product != 0:
                product *= word[1]
            else:
                product = word[1]
            if invProduct != 0:
                invProduct *= (1 - word[1])
            else:
                invProduct = (1 - word[1])
        return product / (product + invProduct)

spamTest = spamFilterProbability([["I", "am", "spam", "spam", "I", "am"], ["I", "do", "not", "like", "that", "spamiam"]], [["do", "i", "like", "green", "eggs", "and", "ham"], ["i", "do"]])
testBody = "I am not spam spamiam please don't delete me"
testBody2 = "I like you and also green eggs and ham"

print("The word probability of 'I':")
print(spamTest.getWordProbability("I"))
print("\nThe word probability of 'Spam':")
print(spamTest.getWordProbability("Spam"))
print("\nThe list of probabilities for testBody:")
print(spamTest.getProbabilityList(testBody.split()))
print("\nThe combined probabilities of testBody (likelihood of it being spam):")
print(spamTest.getCombinedProbability(testBody.split()))

print("\nThe list of probabilities for testBody2:")
print(spamTest.getProbabilityList(testBody2.split()))
print("\nThe combined probabilities of testBody2")
print(spamTest.getCombinedProbability(testBody2.split()))

