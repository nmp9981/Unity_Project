//오일러 피 함수
ll Euler(ll n){
    ll res = n;
    for (ll i = 2; i * i <= n; i++) {
		if (n % i == 0) res = res-(res/i);
		while (n % i == 0) n = n/i;
	}

	if (n > 1) res = res-(res/n);
	return res;
}
