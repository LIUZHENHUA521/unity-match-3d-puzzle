  (func $j43052 (type 3) (param i32 i32 i32)
    (local i32 i32 i32 f32 f32 f32 f32 f32 f32)
    global.get 0
    i32.const 80
    i32.sub
    local.set 2
    local.get 2
    global.set 0
    local.get 0
    i32.load8_u offset=44
    if  ;; label = @1
      local.get 2
      i32.const -64
      i32.sub
      local.get 0
      i32.load offset=28
      i32.const 0
      i32.const 1028
      call_indirect (type 3)
      local.get 0
      f32.load offset=16
      local.set 7
      local.get 0
      f32.load offset=20
      local.set 8
      local.get 2
      f32.load offset=68
      local.get 1
      f32.load offset=4
      f32.neg
      local.get 0
      f32.load offset=36
      f32.mul
      f32.const 0x1.47ae14p-7 (;=0.01;)
      f32.mul
      f32.add
      local.set 6
      local.get 7
      local.get 8
      local.get 6
      local.get 6
      local.get 8
      f32.gt
      select
      local.get 6
      local.get 7
      f32.lt
      select
      local.set 6
      local.get 0
      i32.load offset=28
      local.set 4
      i32.const 4524773
      i32.load8_u
      i32.eqz
      if  ;; label = @2
        i32.const 3439908
        i32.const 870
        call_indirect (type 0)
        i32.const 4524773
        i32.const 1
        i32.store8
      end
      i32.const 3439908
      i32.load
      i32.load offset=92
      local.set 3
      local.get 3
      f32.load offset=32
      local.set 7
      local.get 3
      f32.load offset=24
      local.get 6
      f32.mul
      local.set 8
      local.get 3
      f32.load offset=28
      local.get 6
      f32.mul
      local.set 9
      local.get 2
      local.get 7
      local.get 6
      f32.mul
      f32.store offset=56
      local.get 2
      local.get 2
      i32.load offset=56
      i32.store offset=24
      local.get 2
      local.get 9
      f32.store offset=52
      local.get 2
      local.get 8
      f32.store offset=48
      local.get 2
      i32.load offset=48
      local.set 3
      local.get 2
      i32.load offset=52
      local.set 5
      local.get 2
      local.get 3
      i32.store offset=16
      local.get 2
      local.get 5
      i32.store offset=20
      local.get 4
      local.get 2
      i32.const 16
      i32.add
      i32.const 0
      i32.const 1386
      call_indirect (type 3)
    end
    local.get 0
    i32.load8_u offset=45
    if  ;; label = @1
      local.get 0
      i32.load offset=28
      local.set 4
      local.get 2
      i32.const -64
      i32.sub
      local.get 4
      i32.const 0
      i32.const 2662
      call_indirect (type 3)
      local.get 2
      f32.load offset=72
      local.set 8
      local.get 2
      f32.load offset=68
      local.set 9
      local.get 2
      f32.load offset=64
      local.set 10
      i32.const 4524773
      i32.load8_u
      i32.eqz
      if  ;; label = @2
        i32.const 3439908
        i32.const 870
        call_indirect (type 0)
        i32.const 4524773
        i32.const 1
        i32.store8
      end
      i32.const 3439908
      i32.load
      i32.load offset=92
      local.set 3
      local.get 1
      f32.load
      local.set 6
      local.get 0
      f32.load offset=40
      local.set 7
      local.get 3
      f32.load offset=24
      local.get 6
      f32.mul
      local.get 7
      f32.mul
      local.set 11
      local.get 9
      local.get 3
      f32.load offset=28
      local.get 6
      f32.mul
      local.get 7
      f32.mul
      f32.const 0x1.99999ap-4 (;=0.1;)
      f32.mul
      f32.add
      local.set 9
      local.get 2
      local.get 8
      local.get 3
      f32.load offset=32
      local.get 6
      f32.mul
      local.get 7
      f32.mul
      f32.const 0x1.99999ap-4 (;=0.1;)
      f32.mul
      f32.add
      f32.store offset=40
      local.get 2
      local.get 2
      i32.load offset=40
      i32.store offset=8
      local.get 2
      local.get 9
      f32.store offset=36
      local.get 2
      local.get 10
      local.get 11
      f32.const 0x1.99999ap-4 (;=0.1;)
      f32.mul
      f32.add
      f32.store offset=32
      local.get 2
      i32.load offset=32
      local.set 5
      local.get 2
      i32.load offset=36
      local.set 3
      local.get 2
      local.get 5
      i32.store
      local.get 2
      local.get 3
      i32.store offset=4
      local.get 4
      local.get 2
      i32.const 0
      i32.const 1725
      call_indirect (type 3)
    end
    local.get 2
    i32.const 80
    i32.add
    global.set 0)
