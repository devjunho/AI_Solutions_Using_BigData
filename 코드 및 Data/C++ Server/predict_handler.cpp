#include "predict_handler.h"

DB::DB() { }

DB::~DB() { }

sql::Connection* DB::ConnectDB()                        // 완료
{
    try
    {
        sql::Driver* driver = sql::mariadb::get_driver_instance();
        sql::SQLString url = "jdbc:mariadb://localhost:3306/PREDICT";
        sql::Properties properties({{"user", "JUNHO"}, {"password", "1234"}});
        std::cout << "DB 접속 성공" << std::endl;

        return driver->connect(url, properties);   
     }
    catch(sql::SQLException& e)
    {
        std::cerr << "DB 접속 실패: " << e.what() << std::endl;
        exit(1);
    }
}

void DB::DisconnectDB(sql::Connection* conn)            // 완료
{
    if (!conn->isClosed())
    {
        conn->close();
        std::cout << "DB 접속 해제" << std::endl;
    }
}

Handler::Handler(int sock)                              // 완료
{
    int iSock = sock;
}

Handler::~Handler() { }

void Handler::LoginCheck(Info& info, int sock)
{
    std::string sendData;
    json js;
    int bytesSent = 0;
    try
    {
        DB db;
        sql::Connection*con = db.ConnectDB();
        sql::PreparedStatement*InManageInfo = con->prepareStatement("SELECT NO, NAME, PW FROM USER WHERE ID = ?");
        InManageInfo->setString(1, info.ID);
        sql::ResultSet*Log = InManageInfo->executeQuery();
        while (Log->next())
        {
            info.NO = Log->getInt(1);
            info.NAME = Log->getString(2);
            info.PW = Log->getString(3);
        }

        js = {
            {"Type", SUCCEED},
            {"NO", info.NO},
            {"NAME", info.NAME},
            {"PW", info.PW}
        };

        sendData = js.dump();
        std::cout << sendData << std::endl;

        bytesSent = write(sock, sendData.c_str(), sendData.length());
        std::cout << bytesSent << std::endl;

        return;
    }
    catch (json::parse_error &e)
    {
        std::cerr << "JSON 파싱 에러 : FAIL" << e.what();

        js = json{{"Type", FAIL}};
        sendData = js.dump();
        bytesSent = write(sock, sendData.c_str(), sendData.length());

        std::cout << bytesSent << std::endl;

    }
    catch (json::type_error &e)
    {
        std::cerr << "JSON 타입 에러 : FAIL" << e.what();
        
        js = json{{"Type", FAIL}};
        sendData = js.dump();
        bytesSent = write(sock, sendData.c_str(), sendData.length());

        std::cout << bytesSent << std::endl;
    }
}

void Handler::JoinUser(Info& info, int sock)
{
    std::string sendData;
    json js;
    int bytesSent = 0;
    try
    {
        int count;
        DB db;
        sql::Connection*con = db.ConnectDB();
        sql::PreparedStatement*InManageInfo = con->prepareStatement("SELECT COUNT(*) AS cnt FROM USER WHERE ID = ?");
        InManageInfo->setString(1, info.ID);
        sql::ResultSet*Log = InManageInfo->executeQuery();
        while (Log->next())
        {
            count = Log->getInt(1);
        }
        std::cout << "count: " << count << std::endl;
        if (count != 0)
        {
            js = json{{"Type", DUPLICATE}};
            sendData = js.dump();
            bytesSent = write(sock, sendData.c_str(), sendData.length());
            return;
        }

        DB db2; // 다른 DB 객체를 사용해야 하므로 이름을 변경함
        sql::Connection* con2 = db2.ConnectDB();
        sql::PreparedStatement* InManageInfo2 = con2->prepareStatement("INSERT INTO USER (NAME, ID, PW) VALUES (?, ?, ?)");
        InManageInfo2->setString(1, info.NAME);
        InManageInfo2->setString(2, info.ID);
        InManageInfo2->setString(3, info.PW);
        InManageInfo2->executeUpdate();

        js = {
            {"Type", SUCCEED},
        };

        sendData = js.dump();
        std::cout << sendData << std::endl;

        bytesSent = write(sock, sendData.c_str(), sendData.length());
        std::cout << bytesSent << std::endl;

        return;
    }
    catch (json::parse_error &e)
    {
        std::cerr << "JSON 파싱 에러 : FAIL" << e.what();

        js = json{{"Type", FAIL}};
        sendData = js.dump();
        bytesSent = write(sock, sendData.c_str(), sendData.length());

        std::cout << bytesSent << std::endl;

    }
    catch (json::type_error &e)
    {
        std::cerr << "JSON 타입 에러 : FAIL" << e.what();
        
        js = json{{"Type", FAIL}};
        sendData = js.dump();
        bytesSent = write(sock, sendData.c_str(), sendData.length());

        std::cout << bytesSent << std::endl;
    }
}

void Handler::GraphRead(Info& info, int sock)
{
    std::string sendData;
    json js;
    int bytesSent = 0;
    int count;
    try
    {
        DB db;
        char buffer[1024] = {0};
        sql::Connection*con = db.ConnectDB();
        sql::PreparedStatement*InManageInfo = con->prepareStatement("SELECT COUNT(*) AS total_rows FROM RESULT");
        sql::ResultSet*Log = InManageInfo->executeQuery();
        while (Log->next())
        {
            count = Log->getInt(1);
        }

        std::cout << "Count:" << count << std::endl;    // 확인용

        int i = 0;
        int SexList[count];
        int RiskList[count];

        DB db2;
        sql::Connection*con2 = db2.ConnectDB();
        sql::PreparedStatement*InManageInfo2 = con2->prepareStatement("SELECT SEX, PERCENT_CARDIO FROM RESULT");
        sql::ResultSet*Log2 = InManageInfo2->executeQuery();
        while (Log2->next())
        {
            info.Sex = Log2->getInt(1);
            info.Percent_Cardio = Log2->getInt(2);
            SexList[i] = info.Sex;
            RiskList[i] = info.Percent_Cardio;
            i++;
        }

        js = {
            {"Type", SUCCEED},
            {"Sex", json::array()},
            {"Risk", json::array()}
        };

        for (int j=0; j<count; j++)
        {
            js["SexList"].push_back(SexList[j]);
            js["RiskList"].push_back(RiskList[j]);
        }

        sendData = js.dump();
        std::cout << sendData << std::endl;

        bytesSent = write(sock, sendData.c_str(), sendData.length());
        std::cout << bytesSent << std::endl;

        return;
    }
    catch (json::parse_error &e)
    {
        std::cerr << "JSON 파싱 에러 : FAIL" << e.what();

        js = json{{"Type", FAIL}};
        sendData = js.dump();
        bytesSent = write(sock, sendData.c_str(), sendData.length());

        std::cout << bytesSent << std::endl;

    }
    catch (json::type_error &e)
    {
        std::cerr << "JSON 타입 에러 : FAIL" << e.what();
        
        js = json{{"Type", FAIL}};
        sendData = js.dump();
        bytesSent = write(sock, sendData.c_str(), sendData.length());

        std::cout << bytesSent << std::endl;
    }
}

void Handler::ReceiveInformation(Info& info, int sock)
{
    std::string sendData;
    json js;
    int bytesSent = 0;
    try
    {

        PythonConnect(info);

        DB db;
        sql::Connection* con = db.ConnectDB();

        // Delete existing record
        {
            sql::PreparedStatement* deleteStmt = con->prepareStatement("DELETE FROM RESULT WHERE NO = ?");
            deleteStmt->setInt(1, info.NO);
            deleteStmt->executeUpdate();
            delete deleteStmt;
        }

        // Insert new record
        {
            sql::PreparedStatement* insertStmt = con->prepareStatement("INSERT INTO RESULT VALUES (?, ?, ?, ?)");
            insertStmt->setInt(1, info.NO);
            insertStmt->setString(2, info.ID);
            insertStmt->setInt(3, info.Gender);
            insertStmt->setInt(4, info.Percent_Cardio);
            insertStmt->executeUpdate();
            delete insertStmt;
        }

        js = {
            {"Type", SUCCEED},
            {"Cardio", info.Percent_Cardio}
        };

        sendData = js.dump();
        std::cout << "sendData: " << sendData << std::endl;

        bytesSent = write(sock, sendData.c_str(), sendData.length());
        std::cout << bytesSent << std::endl;

        return;
    }
    catch (json::parse_error &e)
    {
        std::cerr << "JSON 파싱 에러 : FAIL" << e.what();

        js = json{{"Type", FAIL}};
        sendData = js.dump();
        bytesSent = write(sock, sendData.c_str(), sendData.length());

        std::cout << bytesSent << std::endl;

    }
    catch (json::type_error &e)
    {
        std::cerr << "JSON 타입 에러 : FAIL" << e.what();
        
        js = json{{"Type", FAIL}};
        sendData = js.dump();
        bytesSent = write(sock, sendData.c_str(), sendData.length());

        std::cout << bytesSent << std::endl;
    }
}

void Handler::PythonConnect(Info& info)
{
    // JSON 데이터 생성
    json data;
    data["Age"] = info.Age;
    data["Gender"] = info.Gender;
    data["Ap_hi"] = info.Ap_hi;
    data["Ap_lo"] = info.Ap_lo;
    data["Smoke"] = info.Smoke;
    data["Alco"] = info.Alco;
    data["Active"] = info.Active;
    data["Bmi"] = info.Bmi;
    data["Bp"] = info.Bp;

    // JSON 데이터를 문자열로 변환
    std::string json_str = data.dump();

    // TCP 소켓 생성
    int client_socket = socket(AF_INET, SOCK_STREAM, 0);
    if (client_socket == -1)
    {
        std::cerr << "Socket creation failed" << std::endl;
        return;
    }

    // 서버 주소 설정
    struct sockaddr_in server_addr;
    server_addr.sin_family = AF_INET;
    server_addr.sin_port = htons(5001); // 파이썬 서버 포트
    inet_pton(AF_INET, "10.10.21.119", &server_addr.sin_addr); // 파이썬 서버 IP

    // 서버에 연결
    if (connect(client_socket, (struct sockaddr *)&server_addr, sizeof(server_addr)) == -1) 
    {
        std::cerr << "Connection failed" << std::endl;
        close(client_socket);
        return;
    }

    // JSON 데이터 전송
    send(client_socket, json_str.c_str(), json_str.length(), 0);
    std::cout << "C++ 스레드에서 파이썬으로 JSON 데이터 전송 완료" << std::endl;

    // 서버로부터 응답 받기
    char buffer[1024] = {0};
    int bytes_received = recv(client_socket, buffer, sizeof(buffer) - 1, 0);
    if (bytes_received == -1)
    {
        std::cerr << "Error in receiving data" << std::endl;
    }
    else
    {
        std::cout << "파이썬에서 받은 응답:" << buffer << std::endl;
        // 받은 데이터를 문자열로 만들어서 JSON 객체로 파싱
        buffer[bytes_received] = '\0';  // 널 종료 문자 추가
        std::string received_data(buffer);
        
        try
        {
            // JSON 파싱
            json j = json::parse(received_data);
            
            // 필드 값 추출
            info.Type = j["Type"];
            info.Cardio = j["Cardio"];
            
            // 퍼센트 형식으로 변경
            info.Percent_Cardio = round(info.Cardio * 100);

            // 추출한 필드 값 출력
            std::cout << "Type: " << info.Type << std::endl;
            std::cout << "Cardio: " << info.Cardio << std::endl;
            std::cout << "Percent_Cardio: " << info.Percent_Cardio << std::endl;
        }
        catch (const std::exception& e)
        {
            std::cerr << "Error parsing JSON: " << e.what() << std::endl;
        }
    }

    // 소켓 닫기
    close(client_socket);
}