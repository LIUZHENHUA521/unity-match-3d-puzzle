  (func $j39443 (type 5) (param i32 i32 i32) (result i32)
    (local i32 i32)
    i32.const 4505044
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3470972
      i32.const 870
      call_indirect (type 0)
      i32.const 3470980
      i32.const 870
      call_indirect (type 0)
      i32.const 3470988
      i32.const 870
      call_indirect (type 0)
      i32.const 3470968
      i32.const 870
      call_indirect (type 0)
      i32.const 3424976
      i32.const 870
      call_indirect (type 0)
      i32.const 3495712
      i32.const 870
      call_indirect (type 0)
      i32.const 3435416
      i32.const 870
      call_indirect (type 0)
      i32.const 4505044
      i32.const 1
      i32.store8
    end
    i32.const 3435416
    i32.load
    i32.const 871
    call_indirect (type 0)
    local.get 0
    local.set 2
    i32.const 3495712
    i32.load
    call $j19250 ;; 
    local.set 3
    local.get 0
    i32.load offset=860
    local.set 0
    block  ;; label = @1
      local.get 0
      i32.eqz
      if  ;; label = @2
        i32.const 3424976
        i32.load
        i32.const 873
        call_indirect (type 1)
        local.set 0
        local.get 0
        i32.const 3470968
        i32.load
        i32.const 918
        call_indirect (type 4)
        local.get 2
        local.get 0
        i32.store offset=860
        br 1 (;@1;)
      end
      local.get 0
      i32.const 3470980
      i32.load
      i32.const 974
      call_indirect (type 4)
    end
    i32.const 0
    local.set 0
    loop  ;; label = @1
      local.get 3
      i32.load offset=12
      local.get 0
      i32.gt_s
      if  ;; label = @2
        local.get 3
        local.get 0
        i32.const 2
        i32.shl
        i32.add
        i32.load offset=16
        local.set 4
        local.get 1
        local.get 4
        i32.const 0
        i32.const 925
        call_indirect (type 2)
        i32.const 0
        i32.const 2401
        call_indirect (type 2)
        i32.shr_u
        i32.const 1
        i32.and
        if  ;; label = @3
          local.get 2
          i32.load offset=860
          local.get 4
          i32.const 3470972
          i32.load
          i32.const 903
          call_indirect (type 3)
        end
        local.get 0
        i32.const 1
        i32.add
        local.set 0
        br 1 (;@1;)
      end
    end
    local.get 2
    i32.load offset=860
    i32.const 3470988
    i32.load
    i32.const 1497
    call_indirect (type 2)
    return)
