import math
import time
import random

class spamFilterProbability:

    def spamFilterProbability(self, spamCorpus, legitCorpus):
        size = len(spamCorpus)
        self.spamDict = {}
        for i in range(0, size):
            for j in range(0, spamCorpus):
                word = spamCorpus[i][j]
                if word in self.spamDict.keys():
                    self.spamDict[word] += 1
                else:
                    self.spamDict[word] = 1

        self.legitDict = {}
        for i in range(0, size):
            for j in range(0, legitCorpus):
                word = legitCorpus[i][j]
                if word in self.spamDict.keys():
                    self.legitDict[word] += 1
                else:
                    self.legitDict[word] = 1

    def getWordProbability(self, word):
        self.spamDict