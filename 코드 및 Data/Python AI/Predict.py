# from tensorflow.keras.models import Sequential
from tensorflow.keras.models import Sequential, load_model
from tensorflow.keras.layers import Dense
from tensorflow.keras.callbacks import EarlyStopping, ModelCheckpoint
import pandas as pd
import numpy as np
import matplotlib.pyplot as plt
import socket, threading
import json
import math

# 특정 값을 사용하여 result 확률 예측
def predict_result(data, model):
    pred = model.predict(np.array([data]))
    return pred[0][0]

def binder(client_socket, addr, model):
    print('Connected by', addr)
    try:
        data = client_socket.recv(1024)

        if not data:
            print("클라이언트 연결 끊김: ", addr)
        else:
            msg = data.decode('utf-8')
            print('받은 메세지 : ', msg)

            jsonmsg = json.loads(msg)
            print('제이슨 메세지 : ', jsonmsg)

            addition = [jsonmsg["Age"], jsonmsg["Gender"], jsonmsg["Ap_hi"], jsonmsg["Ap_lo"], jsonmsg["Smoke"],
                        jsonmsg["Alco"], jsonmsg["Active"], jsonmsg["Bmi"], jsonmsg["Bp"]]
            print("addition", addition)

            result = predict_result(addition, model)
            print("Prediction result:", result)

            change_result = float(result)

            sendmsg = {"Type": 10, "Cardio": change_result}

            jsonsend = json.dumps(sendmsg)  # Python 객체를 JSON 문자열로 변환
            print("보낸 메세지 :", jsonsend)
            client_socket.sendall(jsonsend.encode())

    except json.JSONDecodeError:
        print("JSON 디코딩 오류 발생")

    except ConnectionResetError:
        print("클라이언트 연결 끊김: ", addr)

    except Exception as e:
        print("예기치 않은 오류 발생: ", e)
    finally:
        client_socket.close()
        print("클라가 연결 끊음")

# main
# 전처리가 완료된 데이터 불러오기
df = pd.read_csv("/home/lms/PycharmProjects/pythonProject/Data/cardio_data_processed_revise3.csv")

# 사용할 속성들에 대한 데이터셋 만들기
cols_train = ['age', 'gender', 'ap_hi', 'ap_lo', 'smoke', 'alco', 'active', 'bmi', 'bp']
X_train_pre = df[cols_train]

# 결과값 데이터
y = df['cardio'].values

# 데이터프레임을 무작위로 섞기
shuffled_df = df.sample(frac=1, random_state=42)  # random_state를 설정하여 재현 가능한 무작위 섞기

# 학습셋과 테스트셋 분리
train_size = int(0.8 * len(shuffled_df))
train_df = shuffled_df[:train_size]
test_df = shuffled_df[train_size:]

# 학습셋과 테스트셋의 입력 데이터와 결과값 나누기
X_train = train_df[cols_train]
y_train = train_df['cardio'].values
X_test = test_df[cols_train]
y_test = test_df['cardio'].values

# 모델 구조 설정
model = Sequential()
model.add(Dense(64, input_dim=X_train.shape[1], activation='relu'))
model.add(Dense(32, activation='relu'))
model.add(Dense(16, activation='relu'))
model.add(Dense(8, activation='relu'))
model.add(Dense(1, activation='sigmoid'))
model.summary()

# 모델을 실행
model.compile(optimizer='adam', loss='mean_squared_error', metrics=['accuracy'])

# 학습 자동 중단 설정
early_stopping_callback = EarlyStopping(monitor='val_loss', patience=20)

# 체크포인트 설정
modelpath = "./data/model/Ch15-house.keras"
checkpointer = ModelCheckpoint(filepath=modelpath, monitor='val_loss', verbose=0, save_best_only=True)

# 학습 실행
history = model.fit(X_train, y_train, validation_split=0.25, epochs=2000, batch_size=32, callbacks=[early_stopping_callback, checkpointer])

# 모델 평가
loss, accuracy = model.evaluate(X_test, y_test)
print(f"테스트셋 손실값 (Mean Squared Error): {loss}")
print(f"테스트셋 정확도 (Accuracy): {accuracy}")

real_prices = []
pred_prices = []
X_num = []

n_iter = 0
Y_prediction = model.predict(X_test).flatten()
for i in range(25):
    real = y_test[i]
    prediction = Y_prediction[i]
    real_prices.append(real)
    pred_prices.append(prediction)
    n_iter = n_iter + 1
    X_num.append(n_iter)

plt.plot(X_num, pred_prices, label='predicted result')
plt.plot(X_num, real_prices, label='real result')
plt.legend()
# plt.show()

server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

server_socket.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)

server_socket.bind(('', 5001))

server_socket.listen()

print("서버 열림!")

try:
    while True:
        client_socket, addr = server_socket.accept()
        thr = threading.Thread(target=binder, args=(client_socket, addr, model))
        thr.start()

except Exception as e:
    print("Server exception:", e)
finally:
    server_socket.close()