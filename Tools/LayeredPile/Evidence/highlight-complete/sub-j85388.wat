  (func $j85388 (type 4) (param i32 i32)
    (local i32 i32)
    i32.const 4505027
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3432484
      i32.const 870
      call_indirect (type 0)
      i32.const 3468528
      i32.const 870
      call_indirect (type 0)
      i32.const 3468532
      i32.const 870
      call_indirect (type 0)
      i32.const 3435416
      i32.const 870
      call_indirect (type 0)
      i32.const 4505027
      i32.const 1
      i32.store8
    end
    i32.const 3435416
    i32.load
    i32.const 871
    call_indirect (type 0)
    block  ;; label = @1
      local.get 0
      i32.const 0
      i32.const 0
      i32.const 924
      call_indirect (type 5)
      if  ;; label = @2
        i32.const 0
        i32.const 14065
        call_indirect (type 1)
        local.set 0
        i32.const 3435416
        i32.load
        i32.const 871
        call_indirect (type 0)
        local.get 0
        i32.const 0
        i32.const 0
        i32.const 924
        call_indirect (type 5)
        br_if 1 (;@1;)
      end
      i32.const 3432484
      i32.load
      i32.const 871
      call_indirect (type 0)
      i32.const 3468528
      i32.load
      drop
      i32.const 3432484
      i32.load
      i32.load offset=92
      i32.load offset=88
      i32.load offset=12
      local.set 3
      i32.const 0
      local.set 1
      loop  ;; label = @2
        local.get 1
        local.get 3
        i32.ge_s
        br_if 1 (;@1;)
        i32.const 3432484
        i32.load
        i32.const 871
        call_indirect (type 0)
        i32.const 3432484
        i32.load
        i32.load offset=92
        i32.load offset=88
        local.get 1
        i32.const 3468532
        i32.load
        i32.const 895
        call_indirect (type 5)
        local.set 2
        i32.const 3435416
        i32.load
        i32.const 871
        call_indirect (type 0)
        local.get 2
        i32.const 0
        i32.const 0
        i32.const 915
        call_indirect (type 5)
        if  ;; label = @3
          local.get 2
          local.get 0
          local.get 1
          i32.const 20234
          call_indirect (type 3)
          local.get 2
          i32.const 1
          i32.store8 offset=772
        end
        local.get 1
        i32.const 1
        i32.add
        local.set 1
        br 0 (;@2;)
      end
      unreachable
    end)
