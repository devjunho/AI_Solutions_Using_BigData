#ifndef DATA_H
#define DATA_H
#include <string>

struct Info
{
int Type;
int Sex;
int Age;
int Gender;
int Ap_hi;
int Ap_lo;
int Smoke;
int Alco;
int Active;
int Bmi;
int Bp;
int Percent_Cardio;
double Cardio;
int NO;
std::string ID;
std::string PW;
std::string NAME;
};

enum TYPE
{ 
    // 0번
    CONNECT_FAIL = 0,

    // 10번
    SUCCEED = 10,

    // 20번
    FAIL = 20,

    // 21번
    DUPLICATE = 21,

    // 30번
    EMPTY = 30,

    // 40번
    LOGIN = 40,

    // 50번
    JOIN = 50,

    // 60번
    SHOWGRAPH = 60,

    // 70번
    SENDINPUTDATA = 70
};

#endif