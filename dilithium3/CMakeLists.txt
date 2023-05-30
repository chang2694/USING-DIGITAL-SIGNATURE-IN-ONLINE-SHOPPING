set(DILITHIUM_SRCS sign.c packing.c polyvec.c poly.c ntt.c reduce.c rounding.c)
set(DILITHIUM_FIPS202_SRCS ${DILITHIUM_SRCS} symmetric-shake.c)
set(DILITHIUM_AES_SRCS ${DILITHIUM_SRCS} symmetric-aes.c)
set(FIPS202_SRCS fips202.c)
set(AES256CTR_SRCS aes256ctr.c)
set(TEST_DILITHIUM_SRCS signature.c randombytes.c)

if(MSVC)
  add_compile_options(/nologo /O2 /W4 /wd4146 /wd4244)
else()
  add_compile_options(-Wall -Wextra -Wpedantic -Werror)
  add_compile_options(-Wmissing-prototypes -Wredundant-decls -Wshadow -Wpointer-arith)
  add_compile_options(-O3 -fomit-frame-pointer)
endif()

add_library(fips202_ref ${FIPS202_SRCS})
add_library(aes256ctr_ref ${AES256CTR_SRCS})

add_library(dilithium3_ref ${DILITHIUM_FIPS202_SRCS})
target_compile_definitions(dilithium3_ref PUBLIC DILITHIUM_MODE=3)
target_link_libraries(dilithium3_ref INTERFACE fips202_ref)

add_executable(test_dilithium3_ref ${TEST_DILITHIUM_SRCS})
target_link_libraries(test_dilithium3_ref dilithium3_ref)

add_test(NAME dilithium3_ref COMMAND test_dilithium3_ref)

add_test(NAME vectors3_ref COMMAND PowerShell -Command "$<TARGET_FILE:test_vectors3_ref> | dos2unix > tvecs3")

add_test(NAME hashes COMMAND sha256sum -c ../../SHA256SUMS)
