#define CONFIG_H

// #define DILITHIUM_MODE 2
// #define DILITHIUM_USE_AES
// #define DILITHIUM_RANDOMIZED_SIGNING
// #define USE_RDPMC
// #define DBENCH

#define CRYPTO_ALGNAME "Dilithium3"
#define DILITHIUM_NAMESPACETOP pqcrystals_dilithium3_ref
#define DILITHIUM_NAMESPACE(s) pqcrystals_dilithium3_ref_##s
