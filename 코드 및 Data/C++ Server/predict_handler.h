#ifndef HANDLER_H
#define HANDLER_H

#include <iostream>
#include <cstdlib>
#include <cstring>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <ctime>
#include <string>
#include <unistd.h>
#include <arpa/inet.h>
#include <regex>
#include <cmath>
#include <mariadb/conncpp.hpp>
#include <nlohmann/json.hpp>
#include "DATA.h"
using json = nlohmann::json;

class DB
{
public:
    DB();
    ~DB();
    sql::Connection* ConnectDB();
    void DisconnectDB(sql::Connection* conn);
};

class Handler
{
public:

    Handler(int sock);
    ~Handler();
    void LoginCheck(Info& info, int sock);
    void JoinUser(Info& info, int sock);
    void GraphRead(Info& info, int sock);
    void ReceiveInformation(Info& info, int sock);
    void PythonConnect(Info& info);

private:

    int iSock;

};

#endif