﻿import keras

class CalculateLoss(keras.callbacks.Callback):
    def on_train_begin(self, logs={}):
        self.losses = []

    def on_batch_end(self, batch, logs={}):
        #self.losses.append(logs.get('loss'))
        print("Loss Callback: " + str(logs.get('loss')))
