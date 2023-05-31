CC ?= /usr/bin/cc
CFLAGS += -Wall -Wextra -Wpedantic -Wmissing-prototypes -Wredundant-decls \
  -Wshadow -Wvla -Wpointer-arith -O3 -fomit-frame-pointer
NISTFLAGS += -Wno-unused-result -O3 -fomit-frame-pointer
SOURCES = sign.c packing.c polyvec.c poly.c ntt.c reduce.c rounding.c
HEADERS = config.h params.h api.h sign.h packing.h polyvec.h poly.h ntt.h \
  reduce.h rounding.h symmetric.h randombytes.h
KECCAK_SOURCES = $(SOURCES) fips202.c symmetric-shake.c
KECCAK_HEADERS = $(HEADERS) fips202.h
AES_SOURCES = $(SOURCES) fips202.c aes256ctr.c symmetric-aes.c
AES_HEADERS = $(HEADERS) fips202.h aes256ctr.h

.PHONY: all speed shared clean

all: \
  signature \

shared: \
  libpqcrystals_dilithium3_ref.so \
  libpqcrystals_fips202_ref.so \
  libpqcrystals_aes256ctr_ref.so

libpqcrystals_fips202_ref.so: fips202.c fips202.h
	$(CC) -shared -fPIC $(CFLAGS) -o $@ $<

libpqcrystals_aes256ctr_ref.so: aes256ctr.c aes256ctr.h
	$(CC) -shared -fPIC $(CFLAGS) -o $@ $<

libpqcrystals_dilithium3_ref.so: $(SOURCES) $(HEADERS) symmetric-shake.c
	$(CC) -shared -fPIC $(CFLAGS) \
	  -o $@ $(SOURCES) symmetric-shake.c

signature: signature.c randombytes.c $(KECCAK_SOURCES) \
  $(KECCAK_HEADERS)
	$(CC) $(CFLAGS) \
	  -o $@ $< randombytes.c $(KECCAK_SOURCES)

clean:
	rm -f *~ *.gcno *.gcda *.lcov
	rm -f libpqcrystals_dilithium3_ref.so
	rm -f libpqcrystals_fips202_ref.so
	rm -f libpqcrystals_aes256ctr_ref.so
	rm -f signature
