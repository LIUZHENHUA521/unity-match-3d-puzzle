  (func $j65259 (type 2) (param i32 i32) (result i32)
    (local i32 f32)
    i32.const 4507777
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3440340
      call $j870
      i32.const 4507777
      i32.const 1
      i32.store8
    end
    local.get 0
    i32.load offset=16
    local.set 1
    block  ;; label = @1
      block  ;; label = @2
        block  ;; label = @3
          local.get 0
          i32.load offset=8
          br_table 0 (;@3;) 1 (;@2;) 2 (;@1;)
        end
        local.get 0
        i32.const -1
        i32.store offset=8
        local.get 1
        f32.load offset=584
        local.set 3
        i32.const 3440340
        i32.load
        call $j873
        local.set 1
        local.get 1
        i32.const -1082130432
        i32.store offset=12
        local.get 1
        local.get 3
        f32.store offset=8
        local.get 0
        local.get 1
        i32.store offset=12
        local.get 0
        i32.const 1
        i32.store offset=8
        i32.const 1
        return
      end
      local.get 0
      i32.const -1
      i32.store offset=8
      i32.const 0
      local.set 0
      loop  ;; label = @2
        local.get 1
        i32.load offset=676
        local.set 2
        local.get 0
        local.get 2
        i32.load offset=12
        i32.ge_s
        br_if 1 (;@1;)
        local.get 2
        local.get 0
        i32.const 2
        i32.shl
        i32.add
        i32.load offset=16
        i32.const 0
        i32.const 0
        call $j2810
        local.get 0
        i32.const 1
        i32.add
        local.set 0
        br 0 (;@2;)
      end
      unreachable
    end
    i32.const 0
    return)
