#include <iostream>
using namespace std;
typedef long long ll;

//확장 유클리드 알고리즘
ll ExpandEuclid(ll a,ll b, ll &x, ll &y){
    if(b==0) {
        x=1;
        y=0;
        return a;
    }
    
    ll x1,y1;
    ll gcd = ExpandEuclid(b, a%b,x1,y1);
    x = y1;
    y = x1-(a/b)*y1;
    return gcd;
}

int main()
{
    ll a,b;
    cin>>a>>b;
    
    ll x,y;
    ll gcd = ExpandEuclid(a, b, x, y);

    std::cout << "gcd(" << a << ", " << b << ") = " << gcd << "\n";
    std::cout << "x = " << x << ", y = " << y << "\n";
    std::cout << "검증: " << a << " * " << x << " + " << b << " * " << y << " = " << (a * x + b * y) << "\n";
    return 0;
}
