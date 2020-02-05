# -*- coding: UTF-8 -*-
from PyQt5 import QtCore, QtGui, QtWidgets
from PyQt5.QtGui import QPixmap
from pose.estimator import TfPoseEstimator
from pose.networks import get_graph_path
from utils.sort import Sort
from utils.actions import actionPredictor
from utils.joint_preprocess import *
import sys
import cv2
import numpy as np
import time
import settings

import logging
import logging.config

poseEstimator = None

#logging.config.fileConfig(fname='file.conf', disable_existing_loggers=False)
#
## Get the logger specified in the file
#logger = logging.getLogger(__name__)



def load_model():
    global poseEstimator
    poseEstimator = TfPoseEstimator(
        get_graph_path('mobilenet_thin'), target_size=(432, 368))


class Ui_MainWindow(QtWidgets.QWidget):

    def __init__(self, parent=None):
        super(Ui_MainWindow, self).__init__(parent)
        self.tracker = Sort(settings.sort_max_age, settings.sort_min_hit)
        self.timer_camera = QtCore.QTimer()
        self.cap = cv2.VideoCapture()
        self.CAM_NUM = 0
        self.set_ui()
        self.slot_init()
        self.__flag_mode = 0
        self.fps = 0.00
        self.data = {}
        self.memory = {}
        self.joints = []
        self.current = []
        self.previous = []

    def set_ui(self):

        ####   main layout  ####
        self.__layout_main = QtWidgets.QHBoxLayout()
        
        
        ####  button layout ####
        self.__layout_fun_button = QtWidgets.QVBoxLayout()
        
        # set buttons #
        self.button_open_camera = QtWidgets.QPushButton(u'Camera OFF') #camera button
        self.button_mode_1 = QtWidgets.QPushButton(u'Openpose OFF') # openpose button
        self.button_mode_2 = QtWidgets.QPushButton(u'Action recognition OFF') # KBlevel button
        self.button_mode_3 = QtWidgets.QPushButton(u'KB level recognition OFF') # action recognition button
        self.button_close = QtWidgets.QPushButton(u'Exit')# exit button

        # set button size #
        self.button_open_camera.setMinimumHeight(50)
        self.button_mode_1.setMinimumHeight(50)
        self.button_mode_2.setMinimumHeight(50)
        self.button_mode_3.setMinimumHeight(50)
        self.button_close.setMinimumHeight(50)
        
        # set button position #
#        self.button_close.move(20, 100)
        
        # set text box #
        self.infoBox = QtWidgets.QTextBrowser(self)
        self.infoBox.setText(u'Design © Copyright 2019 by Haoyu Chen for MMKB at TU Delft and Oulu University')
     
        # set TUDelft logo box #
        self.logolabel1 = QtWidgets.QLabel()
        self.pixmap = QPixmap('tudelft.png')
        self.pixmapnew = self.pixmap.scaled(180, 60)
        self.logolabel1.setPixmap(self.pixmapnew)
        
        # set Oulu logo box #
        self.logolabel2 = QtWidgets.QLabel()
        self.pixmap = QPixmap('oulun_logo.jpeg')
        self.pixmapnew = self.pixmap.scaled(180, 60)
        self.logolabel2.setPixmap(self.pixmapnew)
        
        # set Education logo box #
        self.logolabel3 = QtWidgets.QLabel()
        self.pixmap = QPixmap('education_logo.png')
        self.pixmapnew = self.pixmap.scaled(180, 60)
        self.logolabel3.setPixmap(self.pixmapnew)
                
        # set button position #
#        self.logolabel2.move(20, 250)
        
        # set place for camera #
#        self.copyright = QtWidgets.QPushButton(u'Design © Copyright 2019 by Haoyu Chen for MMKB at TU Delft and Oulu University')
#        self.copyright.setMaximumWidth(150)
#        self.copyright.move(20, 250)
        # put buttons into button layout #
        self.__layout_fun_button.addWidget(self.button_open_camera)
        self.__layout_fun_button.addWidget(self.button_mode_1)
        self.__layout_fun_button.addWidget(self.button_mode_2)
        self.__layout_fun_button.addWidget(self.button_mode_3)
        self.__layout_fun_button.addWidget(self.button_close)
        self.__layout_fun_button.addWidget(self.infoBox)
        self.__layout_fun_button.addWidget(self.logolabel1)    
        self.__layout_fun_button.addWidget(self.logolabel2) 
        self.__layout_fun_button.addWidget(self.logolabel3) 
#        self.__layout_fun_button.addWidget(self.copyright) 
        

#        self.__layout_fun_button.addWidget(self.label_move)

        
        ####  camera widgets ####
        self.label_show_camera = QtWidgets.QLabel()

        self.label_show_camera.setFixedSize(settings.winWidth + 1, settings.winHeight + 1)
        self.label_show_camera.setAutoFillBackground(True)

        ### put layouts into main layout ###
        self.__layout_main.addLayout(self.__layout_fun_button)
        self.__layout_main.addWidget(self.label_show_camera)

    
        self.setLayout(self.__layout_main)
#        self.label_move.raise_()
        self.setWindowTitle(u'Online KB level recognition system (TUDelft & Oulu University)')

    def slot_init(self):
        self.button_open_camera.clicked.connect(self.button_event)
        self.timer_camera.timeout.connect(self.show_camera)

        self.button_mode_1.clicked.connect(self.button_event)
        self.button_mode_2.clicked.connect(self.button_event)
        self.button_mode_3.clicked.connect(self.button_event)
        self.button_close.clicked.connect(self.close)

    def button_event(self):
        sender = self.sender()
        if sender == self.button_mode_1 and self.timer_camera.isActive():
            if self.__flag_mode != 1:
                self.__flag_mode = 1
                self.button_mode_1.setText(u'Openpose ON')
                self.button_mode_2.setText(u'Action recognition OFF')
                self.button_mode_3.setText(u'KB level recognition OFF')
            else:
                self.__flag_mode = 0
                self.button_mode_1.setText(u'Openpose OFF')
                self.infoBox.setText(u'Camera on')
        elif sender == self.button_mode_2 and self.timer_camera.isActive():
            if self.__flag_mode != 2:
                self.__flag_mode = 2
                self.button_mode_1.setText(u'Openpose OFF')
                self.button_mode_2.setText(u'Action recognition ON')
                self.button_mode_3.setText(u'KB level recognition OFF')
            else:
                self.__flag_mode = 0
                self.button_mode_2.setText(u'Action recognition OFF')
                self.infoBox.setText(u'Camera on')
        elif sender == self.button_mode_3 and self.timer_camera.isActive():
            if self.__flag_mode != 3:
                self.__flag_mode = 3
                self.button_mode_1.setText(u'Openpose OFF')
                self.button_mode_2.setText(u'Action recognition OFF')
                self.button_mode_3.setText(u'KB level recognition ON')
            else:
                self.__flag_mode = 0
                self.button_mode_3.setText(u'KB level recognition OFF')
                self.infoBox.setText(u'')
        else:
            self.__flag_mode = 0
            self.button_mode_1.setText(u'Openpose OFF')
            self.button_mode_2.setText(u'Action recognition OFF')
            self.button_mode_3.setText(u'KB level recognition OFF')
            if self.timer_camera.isActive() == False:
                flag = self.cap.open(self.CAM_NUM)
                self.cap.set(cv2.CAP_PROP_FRAME_WIDTH, settings.winWidth)
                self.cap.set(cv2.CAP_PROP_FRAME_HEIGHT, settings.winHeight)
                if flag == False:
                    msg = QtWidgets.QMessageBox.warning(self, u"Warning", u"Camera is not connected",
                                                        buttons=QtWidgets.QMessageBox.Ok,
                                                        defaultButton=QtWidgets.QMessageBox.Ok)
                else:
                    self.timer_camera.start(1)
                    self.button_open_camera.setText(u'Camera ON')
                    self.infoBox.setText(u'Camera connected')
            else:
                self.timer_camera.stop()
                self.cap.release()
                self.label_show_camera.clear()
                self.button_open_camera.setText(u'Camera OFF')
                self.infoBox.setText(u'Camera disconnected')

    def show_camera(self):
        start = time.time()
        ret, frame = self.cap.read()
        show = cv2.resize(frame, (settings.winWidth, settings.winHeight))
        show = cv2.cvtColor(show, cv2.COLOR_BGR2RGB)
        if ret:
            if self.__flag_mode == 1:
                self.infoBox.setText(u'Openpose on')
                humans = poseEstimator.inference(show)
#                print(humans)
                show = TfPoseEstimator.draw_humans(show, humans, imgcopy=False)
                

            elif self.__flag_mode == 2:
                self.infoBox.setText(u'Action recognition on')
                humans = poseEstimator.inference(show)
                ori = np.copy(show)
                show, joints, bboxes, xcenter, sk= TfPoseEstimator.get_skeleton(show, humans, imgcopy=False)
                height = show.shape[0]
                width = show.shape[1]
                if bboxes:
                    result = np.array(bboxes)
                    det = result[:, 0:5]
                    det[:, 0] = det[:, 0] * width
                    det[:, 1] = det[:, 1] * height
                    det[:, 2] = det[:, 2] * width
                    det[:, 3] = det[:, 3] * height
                    trackers = self.tracker.update(det)
                    self.current = [i[-1] for i in trackers]

                    if len(self.previous) > 0:
                        for item in self.previous:
                            if item not in self.current and item in self.data:
                                del self.data[item]
                            if item not in self.current and item in self.memory:
                                del self.memory[item]

                    self.previous = self.current
                    for d in trackers:
                        xmin = int(d[0])
                        ymin = int(d[1])
                        xmax = int(d[2])
                        ymax = int(d[3])
                        label = int(d[4])
                        try:
                            j = np.argmin(np.array([abs(i - (xmax + xmin) / 2.) for i in xcenter]))
                        except:
                            j = 0
                        if joint_filter(joints[j]):
                            joints[j] = joint_completion(joint_completion(joints[j]))
                            if label not in self.data:
                                self.data[label] = [joints[j]]
                                self.memory[label] = 0
                            else:
                                self.data[label].append(joints[j])

                            if len(self.data[label]) == settings.L:
                                pred = actionPredictor().move_status(self.data[label])
                                if pred == 0:
                                    pred = self.memory[label]
                                else:
                                    self.memory[label] = pred
                                self.data[label].pop(0)

                                location = self.data[label][-1][1]
                                if location[0] <= 30:
                                    location = (51, location[1])
                                if location[1] <= 10:
                                    location = (location[0], 31)

                                cv2.putText(show, settings.move_status[pred], (location[0] - 30, location[1] - 10),
                                            cv2.FONT_HERSHEY_SIMPLEX, 0.8,
                                            (0, 255, 0), 2)

                        cv2.rectangle(show, (xmin, ymin), (xmax, ymax),
                                      (int(settings.c[label % 32, 0]),
                                       int(settings.c[label % 32, 1]),
                                       int(settings.c[label % 32, 2])), 4)
                

            elif self.__flag_mode == 3:
                self.infoBox.setText(u'KB level recognition on')

                humans = poseEstimator.inference(show)
                ori = np.copy(show)
                show, joints, bboxes, xcenter, sk= TfPoseEstimator.get_skeleton(show, humans, imgcopy=False)
                height = show.shape[0]
                width = show.shape[1]
                if bboxes:
                    result = np.array(bboxes)
                    det = result[:, 0:5]
                    det[:, 0] = det[:, 0] * width
                    det[:, 1] = det[:, 1] * height
                    det[:, 2] = det[:, 2] * width
                    det[:, 3] = det[:, 3] * height
                    trackers = self.tracker.update(det)
                    self.current = [i[-1] for i in trackers]

                    if len(self.previous) > 0:
                        for item in self.previous:
                            if item not in self.current and item in self.data:
                                del self.data[item]
                            if item not in self.current and item in self.memory:
                                del self.memory[item]

                    self.previous = self.current
                    
                    relation_sum = []
                    
                    for d in trackers:
                        #print(d)
                        xmin = int(d[0])
                        ymin = int(d[1])
                        xmax = int(d[2])
                        ymax = int(d[3])
                        label = int(d[4])
                        mean_x = (xmin + xmax)/2.0
                        relation_sum.append(mean_x)
                        #print('summmm')
                        #print(relation_sum)
                    

                        cv2.rectangle(show, (xmin, ymin), (xmax, ymax),
                                      (int(settings.c[label % 32, 0]),
                                       int(settings.c[label % 32, 1]),
                                       int(settings.c[label % 32, 2])), 4)
                    
                    
                    if len(bboxes) == 1:
                        self.infoBox.setText(u'KB collaborative level: 0 (Only one people detected)')
                        f= open("/Users/chenhaoyu/Desktop/MMdata_stream.txt","a+")
                        #f=open("MMdata_stream.txt","a+")
                        RGB = 0
                        Audio = 0
                        try:
                            f.write("%i %i\n" % (RGB, Audio))
                            f.close()
                        except:
                            f.close()

                    
                    elif len(bboxes) == 2:
                        self.infoBox.setText(u'KB collaborative level: 1')
#                        logger.debug(str())
                        f= open("/Users/chenhaoyu/Desktop/MMdata_stream.txt","a+")
                        #f=open("MMdata_stream.txt","a+")
                        RGB = 1
                        Audio = 0
                        try:
                            f.write("%i %i\n" % (RGB, Audio))
                            f.close()
                        except:
                            f.close()
                        
                            
                        if relation_sum:
                            #print(abs(relation_sum[0] - sum(relation_sum)/len(relation_sum)))
                        
                            if abs(relation_sum[0] - sum(relation_sum)/len(relation_sum))<100:
                                self.infoBox.setText(u'KB collaborative level: 2')
                                f= open("/Users/chenhaoyu/Desktop/MMdata_stream.txt","a+")
                                relation_sum = []
                                RGB = 2
                                Audio = 0
                                try:
                                    f.write("%i %i\n" % (RGB, Audio))
                                    f.close()
                                except:
                                    f.close()
                             
                    else:
                        self.infoBox.setText(u'KB collaborative level: 3')
                        f= open("/Users/chenhaoyu/Desktop/MMdata_stream.txt","a+")
                        RGB = 3
                        Audio = 0
                        try:
                            f.write("%i %i\n" % (RGB, Audio))
                            f.close()
                        except:
                            f.close()
                    
                else:
                    self.infoBox.setText(u'KB collaborative level: None!')
                    f= open("/Users/chenhaoyu/Desktop/MMdata_stream.txt","a+")
                    RGB = 0
                    Audio = 0
                    try:
                        f.write("%i %i\n" % (RGB, Audio))
                        f.close()
                    except:
                        f.close()
                    
                        
            end = time.time()
            self.fps = 1. / (end - start)
            cv2.putText(show, 'FPS: %.2f' % self.fps, (30, 30), cv2.FONT_HERSHEY_SIMPLEX, 0.6, (0, 255, 0), 2)
            showImage = QtGui.QImage(show.data, show.shape[1], show.shape[0], QtGui.QImage.Format_RGB888)
            self.label_show_camera.setPixmap(QtGui.QPixmap.fromImage(showImage))

    def closeEvent(self, event):
        ok = QtWidgets.QPushButton()
        cancel = QtWidgets.QPushButton()

        msg = QtWidgets.QMessageBox(QtWidgets.QMessageBox.Warning, u"Close", u"Close for sure!")

        msg.addButton(ok, QtWidgets.QMessageBox.ActionRole)
        msg.addButton(cancel, QtWidgets.QMessageBox.RejectRole)
        ok.setText(u'Confirm')
        cancel.setText(u'Cancel')
        if msg.exec_() == QtWidgets.QMessageBox.RejectRole:
            event.ignore()
        else:
            if self.cap.isOpened():
                self.cap.release()
            if self.timer_camera.isActive():
                self.timer_camera.stop()
            event.accept()
            print("System exited.")


if __name__ == '__main__':
    load_model()
    print("Load all models done!")
    print("The system starts ro run.")
    app = QtWidgets.QApplication(sys.argv)
    ui = Ui_MainWindow()
    ui.show()
    sys.exit(app.exec_())
