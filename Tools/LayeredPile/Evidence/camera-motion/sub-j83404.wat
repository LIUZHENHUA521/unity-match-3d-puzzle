  (func $j83404 (type 4) (param i32 i32)
    (local i32 i32 i32 i32 i32 i32 i32 i32 i32 f32 f32 f32 f32 f32)
    global.get 0
    i32.const 32
    i32.sub
    local.set 1
    local.get 1
    global.set 0
    i32.const 4509337
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3429624
      i32.const 870
      call_indirect (type 0)
      i32.const 3434312
      i32.const 870
      call_indirect (type 0)
      i32.const 4509337
      i32.const 1
      i32.store8
    end
    i32.const 3434312
    i32.load
    i32.const 871
    call_indirect (type 0)
    block  ;; label = @1
      i32.const 3434312
      i32.load
      i32.load offset=92
      i32.load offset=8
      i32.const 2
      i32.eq
      br_if 0 (;@1;)
      local.get 1
      i32.const 16
      i32.add
      i32.const 0
      i32.const 1860
      call_indirect (type 4)
      local.get 0
      local.set 8
      local.get 1
      f32.load offset=16
      local.set 14
      local.get 1
      i32.load offset=16
      local.set 7
      i32.const 0
      local.set 4
      local.get 7
      local.set 9
      local.get 4
      local.set 7
      local.get 1
      f32.load offset=20
      local.set 15
      local.get 1
      i32.load offset=20
      local.set 2
      i32.const 0
      local.set 4
      i32.const 32
      local.set 5
      local.get 5
      i32.const 31
      i32.and
      local.set 6
      i32.const 32
      local.get 5
      i32.const 63
      i32.and
      i32.le_u
      if  ;; label = @2
        local.get 2
        local.get 6
        i32.shl
        local.set 3
        i32.const 0
        local.set 5
      else
        i32.const 1
        local.get 6
        i32.shl
        i32.const 1
        i32.sub
        local.get 2
        i32.const 32
        local.get 6
        i32.sub
        i32.shr_u
        i32.and
        local.get 4
        local.get 6
        i32.shl
        i32.or
        local.set 3
        local.get 2
        local.get 6
        i32.shl
        local.set 5
      end
      local.get 3
      local.set 2
      local.get 7
      local.set 3
      local.get 9
      local.set 4
      local.get 3
      local.get 2
      i32.or
      local.set 2
      local.get 4
      local.get 5
      i32.or
      local.set 3
      local.get 8
      local.set 4
      local.get 4
      local.get 3
      i32.store offset=220
      local.get 4
      local.get 2
      i32.store offset=224
      local.get 0
      f32.load offset=228
      local.set 13
      block  ;; label = @2
        local.get 13
        f32.const 0x1.4p+4 (;=20;)
        f32.gt
        if  ;; label = @3
          local.get 0
          i32.const 1
          i32.store8 offset=232
          br 1 (;@2;)
        end
        local.get 0
        f32.load offset=216
        local.set 11
        local.get 0
        f32.load offset=212
        local.set 12
        i32.const 4490305
        i32.load8_u
        i32.eqz
        if  ;; label = @3
          i32.const 3434688
          i32.const 870
          call_indirect (type 0)
          i32.const 4490305
          i32.const 1
          i32.store8
        end
        local.get 14
        local.get 12
        f32.sub
        local.set 12
        local.get 15
        local.get 11
        f32.sub
        local.set 11
        i32.const 3434688
        i32.load
        i32.const 871
        call_indirect (type 0)
        local.get 0
        local.get 13
        local.get 12
        local.get 12
        f32.mul
        local.get 11
        local.get 11
        f32.mul
        f32.add
        f32.sqrt
        f32.add
        f32.store offset=228
      end
      i32.const 3429624
      i32.load
      i32.load offset=92
      i32.load
      local.set 7
      local.get 1
      local.set 8
      i32.const 0
      local.set 2
      local.get 0
      f32.load offset=220
      local.get 0
      f32.load offset=212
      f32.sub
      i32.reinterpret_f32
      local.set 10
      local.get 2
      local.set 9
      i32.const 0
      local.set 2
      local.get 0
      f32.load offset=224
      local.get 0
      f32.load offset=216
      f32.sub
      i32.reinterpret_f32
      local.set 3
      i32.const 32
      local.set 5
      local.get 5
      i32.const 31
      i32.and
      local.set 6
      i32.const 32
      local.get 5
      i32.const 63
      i32.and
      i32.le_u
      if  ;; label = @2
        local.get 3
        local.get 6
        i32.shl
        local.set 4
        i32.const 0
        local.set 5
      else
        i32.const 1
        local.get 6
        i32.shl
        i32.const 1
        i32.sub
        local.get 3
        i32.const 32
        local.get 6
        i32.sub
        i32.shr_u
        i32.and
        local.get 2
        local.get 6
        i32.shl
        i32.or
        local.set 4
        local.get 3
        local.get 6
        i32.shl
        local.set 5
      end
      local.get 4
      local.set 3
      local.get 9
      local.set 4
      local.get 10
      local.set 2
      local.get 4
      local.get 3
      i32.or
      local.set 3
      local.get 2
      local.get 5
      i32.or
      local.set 4
      local.get 8
      local.set 2
      local.get 2
      local.get 4
      i32.store
      local.get 2
      local.get 3
      i32.store offset=4
      local.get 1
      local.set 2
      local.get 2
      local.get 4
      i32.store offset=8
      local.get 2
      local.get 3
      i32.store offset=12
      local.get 7
      local.get 2
      i32.const 0
      call $j43052
      local.get 0
      i32.load offset=220
      local.set 3
      local.get 0
      i32.load offset=224
      local.set 2
      local.get 0
      local.get 3
      i32.store offset=212
      local.get 0
      local.get 2
      i32.store offset=216
      local.get 0
      i32.load8_u offset=232
      i32.eqz
      if  ;; label = @2
        local.get 0
        i32.const 1
        local.get 0
        i32.const 13231
        call_indirect (type 3)
        br 1 (;@1;)
      end
      local.get 0
      i32.const 1
      local.get 0
      i32.const 13226
      call_indirect (type 3)
    end
    local.get 1
    i32.const 32
    i32.add
    global.set 0)
