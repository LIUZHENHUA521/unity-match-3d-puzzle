  (func $j85384 (type 4) (param i32 i32)
    (local i32)
    i32.const 4505026
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3452644
      i32.const 870
      call_indirect (type 0)
      i32.const 3452648
      i32.const 870
      call_indirect (type 0)
      i32.const 3432484
      i32.const 870
      call_indirect (type 0)
      i32.const 3468516
      i32.const 870
      call_indirect (type 0)
      i32.const 3468524
      i32.const 870
      call_indirect (type 0)
      i32.const 4505026
      i32.const 1
      i32.store8
    end
    block  ;; label = @1
      local.get 0
      i32.load offset=416
      local.set 1
      local.get 1
      i32.eqz
      br_if 0 (;@1;)
      loop  ;; label = @2
        local.get 2
        local.get 1
        i32.load offset=12
        i32.ge_s
        br_if 1 (;@1;)
        local.get 2
        local.get 1
        local.get 2
        i32.const 164
        i32.mul
        i32.add
        i32.const 16
        i32.add
        i32.load offset=72
        local.get 2
        i32.const 5909
        call_indirect (type 3)
        local.get 2
        local.get 0
        i32.load offset=416
        local.get 2
        i32.const 164
        i32.mul
        i32.add
        i32.const 16
        i32.add
        i32.load offset=76
        local.get 2
        i32.const 5909
        call_indirect (type 3)
        local.get 2
        local.get 0
        i32.load offset=416
        local.get 2
        i32.const 164
        i32.mul
        i32.add
        i32.const 16
        i32.add
        i32.load offset=80
        local.get 2
        i32.const 5909
        call_indirect (type 3)
        local.get 2
        local.get 0
        i32.load offset=416
        local.get 2
        i32.const 164
        i32.mul
        i32.add
        i32.const 16
        i32.add
        i32.load offset=84
        local.get 2
        i32.const 5909
        call_indirect (type 3)
        local.get 2
        local.get 0
        i32.load offset=416
        local.get 2
        i32.const 164
        i32.mul
        i32.add
        i32.const 16
        i32.add
        i32.load offset=88
        local.get 2
        i32.const 5909
        call_indirect (type 3)
        local.get 2
        local.get 0
        i32.load offset=416
        local.get 2
        i32.const 164
        i32.mul
        i32.add
        i32.const 16
        i32.add
        i32.load offset=92
        local.get 2
        i32.const 5909
        call_indirect (type 3)
        local.get 2
        i32.const 1
        i32.add
        local.set 2
        local.get 0
        i32.load offset=416
        local.set 1
        br 0 (;@2;)
      end
      unreachable
    end
    local.get 2
    local.get 0
    local.get 2
    i32.const 13405
    call_indirect (type 2)
    local.get 2
    i32.const 5240
    call_indirect (type 3)
    local.get 2
    local.get 0
    local.get 2
    i32.const 9987
    call_indirect (type 2)
    local.get 2
    i32.const 5240
    call_indirect (type 3)
    local.get 2
    local.get 0
    local.get 2
    i32.const 6736
    call_indirect (type 2)
    local.get 2
    i32.const 5240
    call_indirect (type 3)
    local.get 2
    local.get 0
    local.get 2
    i32.const 8123
    call_indirect (type 2)
    local.get 2
    i32.const 5240
    call_indirect (type 3)
    local.get 2
    local.get 0
    local.get 2
    i32.const 9988
    call_indirect (type 2)
    local.get 2
    i32.const 5240
    call_indirect (type 3)
    local.get 2
    local.get 0
    local.get 2
    i32.const 8124
    call_indirect (type 2)
    local.get 2
    i32.const 5240
    call_indirect (type 3)
    local.get 2
    local.get 0
    local.get 2
    call $j13406 ;; HighlightEffect.get_fxMatBlurOutline
    local.get 2
    i32.const 5240
    call_indirect (type 3)
    i32.const 3432484
    i32.load
    i32.const 871
    call_indirect (type 0)
    i32.const 3432484
    i32.load
    i32.load offset=92
    i32.load offset=88
    local.get 0
    i32.const 3468516
    i32.load
    i32.const 1235
    call_indirect (type 5)
    if  ;; label = @1
      i32.const 3432484
      i32.load
      i32.const 871
      call_indirect (type 0)
      i32.const 3432484
      i32.load
      i32.load offset=92
      i32.load offset=88
      local.get 0
      i32.const 3468524
      i32.load
      i32.const 1342
      call_indirect (type 5)
      drop
    end
    i32.const 3432484
    i32.load
    i32.const 871
    call_indirect (type 0)
    i32.const 3432484
    i32.load
    i32.load offset=92
    i32.load offset=124
    local.get 0
    i32.load offset=864
    i32.const 3452644
    i32.load
    i32.const 1065
    call_indirect (type 5)
    if  ;; label = @1
      i32.const 3432484
      i32.load
      i32.const 871
      call_indirect (type 0)
      i32.const 3432484
      i32.load
      i32.load offset=92
      i32.load offset=124
      local.get 0
      i32.load offset=864
      i32.const 3452648
      i32.load
      i32.const 1873
      call_indirect (type 5)
      drop
    end)
