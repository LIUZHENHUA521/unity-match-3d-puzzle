  (func $j95668 (type 4) (param i32 i32)
    (local i32)
    i32.const 4508663
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3428556
      i32.const 870
      call_indirect (type 0)
      i32.const 3428552
      i32.const 870
      call_indirect (type 0)
      i32.const 3482396
      i32.const 870
      call_indirect (type 0)
      i32.const 4508663
      i32.const 1
      i32.store8
    end
    i32.const 3428556
    i32.load
    i32.const 871
    call_indirect (type 0)
    i32.const 3428556
    i32.load
    i32.load offset=92
    i32.load offset=28
    local.set 1
    i32.const 3428552
    i32.load
    i32.const 873
    call_indirect (type 1)
    local.set 2
    local.get 2
    local.get 0
    i32.const 3482396
    i32.load
    i32.const 0
    i32.const 988
    call_indirect (type 6)
    local.get 1
    local.get 2
    i32.const 0
    i32.const 1061
    call_indirect (type 5)
    local.set 0
    local.get 0
    i32.const 3428552
    i32.load
    i32.const 919
    call_indirect (type 2)
    local.set 1
    i32.const 3428556
    i32.load
    i32.load offset=92
    local.get 1
    i32.store offset=28
    i32.const 3428556
    i32.load
    i32.load offset=92
    drop
    local.get 0
    i32.const 3428552
    i32.load
    i32.const 919
    call_indirect (type 2)
    drop)