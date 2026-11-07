  (func $j36729 (type 2) (param i32 i32) (result i32)
    local.get 0
    i32.load offset=20
    i32.const 0
    call $j7845 ;; Rigidbody.get_isKinematic
    i32.const 1
    i32.xor
    return)
