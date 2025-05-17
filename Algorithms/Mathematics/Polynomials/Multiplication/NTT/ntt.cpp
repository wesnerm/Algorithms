// BOTH GROUP OF SETTINGS ACTUALLY SLOWED DOWN CODE

//#pragma GCC optimize("Ofast")
//#pragma GCC target("sse,sse2,sse3,ssse3,sse4,popcnt,abm,mmx,avx,tune=native")

//#pragma GCC target ("avx2")
//#pragma GCC optimization ("O3")
//#pragma GCC optimization ("unroll-loops")

// https://www.codechef.com/SEPT19A/problems/PSUM

#include <bits/stdc++.h>
#include <algorithm>
#include <iomanip>
#include <istream>
#include <map>
#include <numeric>
#include <ostream>
#include <set>
#include <sstream>
#include <string>
#include <utility>
#include <vector>
using namespace std;
#include <stdio.h>

#define long long long

int N, S, K;
const int MOD = 998244353;
int fact[2002];
int ifact[2002];
int C[101];
int V[101];

int inverse(int a)
{
	int b = MOD, p = 1, q = 0;
	while (b > 0)
	{
		int c = a / b, d = a;
		a = b;
		b = d % b;
		d = p;
		p = q;
		q = d - c * q;
	}
	return p < 0 ? p + MOD : p;
}

void pairsort(int a[], int b[], int n)
{
	pair<int, int> pairt[n];

	for (int i = 0; i < n; i++)
	{
		pairt[i].first = a[i];
		pairt[i].second = b[i];
	}

	sort(pairt, pairt + n);

	for (int i = 0; i < n; i++)
	{
		a[i] = pairt[i].first;
		b[i] = pairt[i].second;
	}
}

void chomp()
{
	while (N > 0 && C[N - 1] > S) N--;
}

long fix(long v)
{
	v %= MOD;
	return (v >= 0) ? v : v + MOD;
}

long sums[101][4096] = {};
long sumslen[101] = {};
bool good[101] = {};

void buildfact()
{
	long factor = 1LL;
	fact[0] = factor;
	for (int i = 1; i < 2001; i++)
		fact[i] = factor = factor * i % MOD;
	factor = inverse(factor);
	for (int i = 2000; i >= 0; i--)
	{
		ifact[i] = factor;
		factor = factor * i % MOD;
	}
}

long factorPoly[4096] = {};
long values[101] = {};
long factors[101] = {};
int halfpoint;
long halfresult;

static long modPow(long n, long p)
{
	long b = n;
	long result = 1;
	while (p != 0)
	{
		if ((p & 1) != 0)
			result = (result * b) % MOD;
		p >>= 1;
		b = (b * b) % MOD;
	}
	return result;
}

void buildHalfpoint(bool aggressive)
{
	halfpoint = N;
	if (true)
	{
		while (halfpoint > 0 && 2 * C[halfpoint - 1] > S)
			halfpoint--;

		if (aggressive)
		{
			if (halfpoint < N)
				while (halfpoint > 0 && C[halfpoint - 1] + C[halfpoint] > S)
					halfpoint--;
		}

		for (int i = halfpoint; i < N; i++)
			for (int j = i + 1; j < N; j++)
			{
				int c = C[i] + C[j];
				if (c > S) break;
				halfresult += modPow(V[i] + V[j], K);
				if (halfresult >= MOD) halfresult -= MOD;
			}
	}
}

void powerSeries(long value, int K)
{
	long factor = 1;
	for (int k = 0; k <= K; k++, factor = factor * value % MOD)
		factorPoly[k] = ifact[k] * factor % MOD;
}

int Reverse(int value)
{
	unsigned n = (unsigned)value;
	n = n >> 16 | n << 16;
	n = n >> 0x8 & 0x00ff00ff | n << 0x8 & 0xff00ff00;
	n = n >> 0x4 & 0x0f0f0f0f | n << 0x4 & 0xf0f0f0f0;
	n = n >> 0x2 & 0x33333333 | n << 0x2 & 0xcccccccc;
	n = n >> 0x1 & 0x55555555 | n << 0x1 & 0xaaaaaaaa;
	return (int)n;
}

inline long Modh(long a, long M, int h)
{
	const long mask = (1LL << 31) - 1;
	long r = a - ((M * (a & mask) >> 31) + M * (a >> 31) >> h - 31) * MOD;
	return r < MOD ? r : r - MOD;
}

int g = 3;

long Invl(long a, long mod)
{
	long b = mod;
	long p = 1, q = 0;
	while (b > 0)
	{
		long c = a / b;
		long d;
		d = a;
		a = b;
		b = d % b;
		d = p;
		p = q;
		q = d - c * q;
	}
	return p < 0 ? p + mod : p;
}

inline int Log2(long value)
{
	double f = ((unsigned long)value); // +.5 -> -1 for zero
	return (((int*)& f)[1] >> 20) - 1023;
}

inline 	int HighestOneBit(int n)
{
	return n != 0 ? 1 << Log2(n) : 0;
}


void NttCore(long* dst, int n, bool inverse)
{
	int h = __builtin_ctz(n); // Log2(n & -n);
	//int h = Log2(n & -n);
	//long b = (1LL << (64 - __builtin_clzll((long)MOD))) << 1;  <-- This failed to work

	// These could be made const with constexpr
	long b = HighestOneBit((long)MOD) << 1;
	int H = __builtin_ctz(b) * 2; // Log2(b) * 2;
	long m = b * b / MOD;

	int wws[1 << h - 1];
	long dw = inverse ? modPow(g, MOD - 1 - (MOD - 1) / n) : modPow(g, (MOD - 1) / n);
	long w = (1LL << 32) % MOD;
	for (int k = 0; k < 1 << h - 1; k++)
	{
		wws[k] = (int)w;
		w = Modh(w * dw, m, H);
	}
	long J = Invl(MOD, 1LL << 32);
	for (int i = 0; i < h; i++)
	{
		for (int j = 0; j < 1 << i; j++)
		{
			int hlimit = 1 << h - i - 1;
			for (int k = 0, s = j << h - i, t = s | hlimit; k < hlimit; k++, s++, t++)
			{
				long u = (dst[s] - dst[t] + 2 * MOD) * wws[k];
				dst[s] += dst[t];
				if (dst[s] >= 2 * MOD) dst[s] -= 2 * MOD;
				long Q = ((long)((unsigned long)((u << 32) * J) >> 32));
				dst[t] = (u >> 32) - (Q * MOD >> 32) + MOD;
			}
		}
		if (i < h - 1)
		{
			for (int k = 0; k < 1 << h - i - 2; k++)
				wws[k] = wws[k * 2];
		}
	}
	for (int i = 0; i < n; i++)
	{
		if (dst[i] >= MOD)
			dst[i] -= MOD;
	}
	for (int i = 0; i < n; i++)
	{
		int rev = ((int)((unsigned int)Reverse(i) >> -h));
		if (i < rev)
		{
			long d = dst[i];
			dst[i] = dst[rev];
			dst[rev] = d;
		}
	}

	if (inverse)
	{
		long inv = Invl(n, MOD);
		for (int i = 0; i < n; i++)
			dst[i] = Modh(dst[i] * inv, m, H);
	}
}

long fa[4096];
long fb[4096];

void convoluteOld(long* a, long* b, long* result, int size);

void convolute(long* a, long* b, long* result, int size) {
	//convoluteOld(a, b, result, size);
	//return;

	copy(a, a + size, fa);
	copy(b, b + size, fb);

	int n = 1;
	while (n < 2 * size) n <<= 1;

	fill(fa + size, fa + n, 0);
	fill(fb + size, fb + n, 0);

	NttCore(fa, n, false);
	NttCore(fb, n, false);
	for (int i = 0; i < n; i++)
		fa[i] = fa[i] * fb[i] % MOD;
	NttCore(fa, n, true);

	for (int i = 0; i < size; i++)
	{
		long t = result[i] + fa[i];
		result[i] = t % MOD;
	}
}

void convoluteOld(long* a, long* b, long* result, int size)
{
	for (int i = 0; i < size; i++)
		for (int j = min(size - i, size) - 1; j >= 0; j--)
			result[i + j] = (result[i + j] + a[i] * b[j]) % MOD;
}


void addTo(long* a, long* b, int size)
{
	for (int i = 0; i < size; i++)
	{
		long t = a[i] + b[i];
		if (t >= MOD) t -= MOD;
		a[i] = t;
	}
}

long solve()
{
	buildHalfpoint(false);

	sums[0][0] = 1;

	int costsSoFar = 0;
	for (int iv = 0; iv < N; iv++)
	{
		int cost = C[iv];
		int value = V[iv];
		if (iv >= halfpoint)
		{
			for (int c = min(costsSoFar, S - cost); c >= 0; c--)
				if (sums[c][0] != 0)
				{
					int halfcount = N - halfpoint;
					while (halfcount > 1 && C[halfpoint + halfcount - 1] + c > S)
						halfcount--;

					for (int i = 0; i < halfcount; i++)
					{
						values[i] = V[i + halfpoint];
						factors[i] = 1;
					}

					for (int i = 0; i <= K; i++)
					{
						long sumfactor = 0;
						for (int j = 0; j < halfcount; j++) sumfactor += factors[j];
						factorPoly[i] = sumfactor % MOD * ifact[i] % MOD;
						for (int j = 0; j < halfcount; j++)
							factors[j] = factors[j] * values[j] % MOD;
					}

					if (c != 0)
						convolute(factorPoly, sums[c], sums[c + cost], K + 1);
					else
						addTo(sums[c + cost], factorPoly, K + 1);
				}
			break;
		}

		powerSeries(value, K);

		for (int c = min(costsSoFar, S - cost); c >= 0; c--)
			if (sums[c][0] != 0)
			{
				if (c != 0)
					convolute(factorPoly, sums[c], sums[c + cost], K + 1);
				else
					addTo(sums[c + cost], factorPoly, K + 1);
			}
		costsSoFar += cost;
	}

	long total = 0;
	for (int i = 1; i <= S; i++)
		if (sums[i] != NULL)
			total += sums[i][K];

	total = total % MOD * fact[K] + halfresult;
	return fix(total);
}

int main(int argc, char** argv)
{
	cin >> N >> S >> K;
	for (int i = 0; i < N; i++)
		cin >> C[i] >> V[i];

	pairsort(C, V, N);

	buildfact();

	long result = solve();
	cout << result;
	return 0;
}

