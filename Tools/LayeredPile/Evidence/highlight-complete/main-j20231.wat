  (func $j20231 (type 1) (param i32 i32 i32)
    (local i32 i32 i32 i32 i32 i32)
    i32.const 4505043
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3485580
      call $j870 ;; 
      i32.const 3485628
      call $j870 ;; 
      i32.const 3485632
      call $j870 ;; 
      i32.const 3432484
      call $j870 ;; 
      i32.const 3428336
      call $j870 ;; 
      i32.const 3435416
      call $j870 ;; 
      i32.const 3427916
      call $j870 ;; 
      i32.const 3437460
      call $j870 ;; 
      i32.const 4505043
      i32.const 1
      i32.store8
    end
    local.get 1
    i32.eqz
    if  ;; label = @1
      i32.const 3427916
      i32.load
      i32.const 0
      call $j888 ;; 
      local.set 1
    end
    local.get 0
    i32.const 416
    i32.add
    local.set 2
    local.get 0
    i32.load offset=416
    local.set 3
    block  ;; label = @1
      block  ;; label = @2
        local.get 3
        i32.eqz
        if  ;; label = @3
          local.get 1
          i32.load offset=12
          local.set 4
          br 1 (;@2;)
        end
        local.get 1
        i32.load offset=12
        local.set 4
        local.get 4
        local.get 3
        i32.load offset=12
        i32.le_s
        br_if 1 (;@1;)
      end
      local.get 2
      i32.const 3428336
      i32.load
      local.get 4
      call $j888 ;; 
      i32.store
    end
    local.get 0
    i32.const 0
    i32.store offset=420
    loop  ;; label = @1
      local.get 1
      i32.load offset=12
      local.get 7
      i32.gt_s
      if  ;; label = @2
        local.get 0
        i32.load offset=416
        local.get 0
        i32.load offset=420
        i32.const 164
        i32.mul
        i32.add
        i32.const 16
        i32.add
        i32.const 0
        call $j19524 ;; ModelMaterials.Init
        local.get 1
        local.get 7
        i32.const 2
        i32.shl
        i32.add
        i32.load offset=16
        local.set 4
        block  ;; label = @3
          block  ;; label = @4
            local.get 0
            i32.load offset=24
            i32.const 1
            i32.eq
            br_if 0 (;@4;)
            local.get 0
            i32.load offset=32
            local.set 3
            block  ;; label = @5
              local.get 3
              if  ;; label = @6
                i32.const 0
                local.set 5
                local.get 3
                i32.load offset=8
                br_if 1 (;@5;)
              end
              i32.const 1
              local.set 5
            end
            local.get 5
            br_if 0 (;@4;)
            local.get 4
            i32.const 0
            call $j1073 ;; 
            local.get 0
            i32.load offset=32
            i32.const 0
            call $j2043 ;; 
            i32.eqz
            br_if 1 (;@3;)
          end
          local.get 0
          i32.load offset=416
          local.get 0
          i32.load offset=420
          i32.const 164
          i32.mul
          i32.add
          i32.const 16
          i32.add
          local.get 4
          i32.store offset=60
          local.get 0
          i32.load offset=416
          local.get 0
          i32.load offset=420
          i32.const 164
          i32.mul
          i32.add
          i32.const 16
          i32.add
          local.get 4
          i32.const 0
          call $j7852 ;; 
          i32.store8 offset=48
          local.get 4
          i32.const 0
          call $j944 ;; 
          local.set 3
          local.get 0
          i32.load offset=424
          local.set 6
          i32.const 3435416
          i32.load
          call $j871 ;; 
          block  ;; label = @4
            local.get 3
            local.get 6
            i32.const 0
            call $j915 ;; 
            i32.eqz
            br_if 0 (;@4;)
            local.get 4
            i32.const 3485580
            i32.load
            call $j1084 ;; 
            local.set 3
            i32.const 3435416
            i32.load
            call $j871 ;; 
            local.get 3
            i32.const 0
            i32.const 0
            call $j915 ;; 
            i32.eqz
            br_if 0 (;@4;)
            local.get 3
            i32.const 0
            call $j2827 ;; 
            i32.eqz
            br_if 0 (;@4;)
            i32.const 20445
            local.get 3
            local.get 0
            i32.load8_u offset=57
            local.get 0
            call $redir_viii
            br 1 (;@3;)
          end
          block  ;; label = @4
            local.get 0
            i32.load offset=320
            local.set 3
            local.get 3
            i32.eqz
            br_if 0 (;@4;)
            local.get 3
            i32.load offset=12
            local.get 3
            i32.load offset=32
            local.get 4
            local.get 3
            i32.load offset=20
            call $redir_iiii
            br_if 0 (;@4;)
            local.get 0
            local.get 0
            i32.load offset=420
            i32.const 1
            i32.add
            i32.store offset=420
            br 1 (;@3;)
          end
          local.get 2
          i32.load
          local.get 0
          i32.load offset=420
          i32.const 164
          i32.mul
          i32.add
          i32.const 16
          i32.add
          i32.const 0
          i32.store8 offset=160
          local.get 4
          i32.const 3437460
          i32.load
          call $j906 ;; 
          local.set 3
          local.get 2
          i32.load
          local.get 0
          i32.load offset=420
          i32.const 164
          i32.mul
          i32.add
          i32.const 16
          i32.add
          local.get 3
          i32.const 0
          i32.ne
          i32.store8 offset=64
          local.get 2
          i32.load
          local.set 8
          local.get 0
          i32.load offset=420
          local.set 5
          local.get 8
          local.get 5
          i32.const 164
          i32.mul
          i32.add
          i32.const 16
          i32.add
          local.set 6
          block  ;; label = @4
            local.get 3
            if  ;; label = @5
              local.get 8
              local.get 5
              i32.const 164
              i32.mul
              i32.add
              i32.const 16
              i32.add
              local.set 6
              i32.const 1
              local.set 5
              br 1 (;@4;)
            end
            local.get 0
            i32.load offset=52
            local.set 5
          end
          local.get 6
          local.get 5
          i32.store offset=68
          local.get 2
          i32.load
          local.get 0
          i32.load offset=420
          i32.const 164
          i32.mul
          i32.add
          i32.const 16
          i32.add
          local.set 6
          local.get 6
          i32.load8_u offset=160
          if  ;; label = @4
            i32.const 0
            local.set 5
          else
            local.get 6
            i32.load offset=68
            i32.const 1
            i32.eq
            local.set 5
          end
          block  ;; label = @4
            local.get 5
            i32.eqz
            if  ;; label = @5
              local.get 0
              i32.load8_u offset=36
              i32.eqz
              br_if 1 (;@4;)
            end
            local.get 0
            local.get 0
            call $j39452 ;; HighlightEffect.CheckCommandBuffers
          end
          block  ;; label = @4
            local.get 3
            if  ;; label = @5
              local.get 2
              i32.load
              local.set 6
              local.get 0
              i32.load offset=420
              local.set 3
              local.get 6
              local.get 3
              i32.const 164
              i32.mul
              i32.add
              i32.const 16
              i32.add
              local.get 4
              i32.const 3437460
              i32.load
              call $j912 ;; 
              i32.const 0
              call $j12455 ;; 
              i32.store offset=52
              br 1 (;@4;)
            end
            block  ;; label = @5
              i32.const 0
              call $j1337 ;; 
              i32.eqz
              br_if 0 (;@5;)
              local.get 4
              i32.const 0
              call $j35347 ;; 
              i32.eqz
              br_if 0 (;@5;)
              local.get 4
              i32.const 3485628
              i32.load
              call $j1084 ;; 
              local.set 3
              i32.const 3435416
              i32.load
              call $j871 ;; 
              local.get 3
              i32.const 0
              i32.const 0
              call $j915 ;; 
              i32.eqz
              br_if 1 (;@4;)
              local.get 2
              i32.load
              local.set 5
              local.get 0
              i32.load offset=420
              local.set 6
              local.get 5
              local.get 6
              i32.const 164
              i32.mul
              i32.add
              i32.const 16
              i32.add
              i32.const 43526
              local.get 3
              i32.const 0
              call $redir_iii
              i32.store offset=52
              br 1 (;@4;)
            end
            local.get 4
            i32.const 3485632
            i32.load
            call $j1084 ;; 
            local.set 3
            i32.const 3435416
            i32.load
            call $j871 ;; 
            local.get 3
            i32.const 0
            i32.const 0
            call $j915 ;; 
            i32.eqz
            br_if 0 (;@4;)
            local.get 2
            i32.load
            local.get 0
            i32.load offset=420
            i32.const 164
            i32.mul
            i32.add
            i32.const 16
            i32.add
            local.get 3
            i32.const 0
            call $j9785 ;; 
            i32.store offset=52
          end
          local.get 2
          i32.load
          local.get 0
          i32.load offset=420
          i32.const 164
          i32.mul
          i32.add
          i32.const 16
          i32.add
          i32.load offset=52
          local.set 3
          i32.const 3435416
          i32.load
          call $j871 ;; 
          local.get 3
          i32.const 0
          i32.const 0
          call $j924 ;; 
          br_if 0 (;@3;)
          local.get 2
          i32.load
          local.get 0
          i32.load offset=420
          i32.const 164
          i32.mul
          i32.add
          i32.const 16
          i32.add
          local.get 4
          i32.const 0
          call $j944 ;; 
          i32.store offset=4
          i32.const 3432484
          i32.load
          call $j871 ;; 
          local.get 0
          i32.const 3432484
          i32.load
          i32.load offset=92
          i32.load
          local.get 2
          i32.load
          local.get 0
          i32.load offset=420
          i32.const 164
          i32.mul
          i32.add
          i32.const 88
          i32.add
          local.get 2
          i32.load
          local.get 0
          i32.load offset=420
          i32.const 164
          i32.mul
          i32.add
          i32.const 16
          i32.add
          i32.load offset=52
          local.get 0
          call $j6737 ;; HighlightEffect.Fork
          local.get 0
          i32.const 3432484
          i32.load
          i32.load offset=92
          i32.load offset=8
          local.get 2
          i32.load
          local.get 0
          i32.load offset=420
          i32.const 164
          i32.mul
          i32.add
          i32.const 96
          i32.add
          local.get 2
          i32.load
          local.get 0
          i32.load offset=420
          i32.const 164
          i32.mul
          i32.add
          i32.const 16
          i32.add
          i32.load offset=52
          local.get 0
          call $j6737 ;; HighlightEffect.Fork
          local.get 0
          i32.const 3432484
          i32.load
          i32.load offset=92
          i32.load offset=12
          local.get 2
          i32.load
          local.get 0
          i32.load offset=420
          i32.const 164
          i32.mul
          i32.add
          i32.const 100
          i32.add
          local.get 2
          i32.load
          local.get 0
          i32.load offset=420
          i32.const 164
          i32.mul
          i32.add
          i32.const 16
          i32.add
          i32.load offset=52
          local.get 0
          call $j6737 ;; HighlightEffect.Fork
          local.get 0
          i32.const 3432484
          i32.load
          i32.load offset=92
          i32.load offset=16
          local.get 2
          i32.load
          local.get 0
          i32.load offset=420
          i32.const 164
          i32.mul
          i32.add
          i32.const 104
          i32.add
          local.get 2
          i32.load
          local.get 0
          i32.load offset=420
          i32.const 164
          i32.mul
          i32.add
          i32.const 16
          i32.add
          i32.load offset=52
          local.get 0
          call $j6737 ;; HighlightEffect.Fork
          local.get 0
          i32.const 3432484
          i32.load
          i32.load offset=92
          i32.load offset=28
          local.get 2
          i32.load
          local.get 0
          i32.load offset=420
          i32.const 164
          i32.mul
          i32.add
          i32.const 108
          i32.add
          local.get 2
          i32.load
          local.get 0
          i32.load offset=420
          i32.const 164
          i32.mul
          i32.add
          i32.const 16
          i32.add
          i32.load offset=52
          local.get 0
          call $j6737 ;; HighlightEffect.Fork
          local.get 0
          i32.const 3432484
          i32.load
          i32.load offset=92
          i32.load offset=4
          local.get 2
          i32.load
          local.get 0
          i32.load offset=420
          i32.const 164
          i32.mul
          i32.add
          i32.const 92
          i32.add
          local.get 2
          i32.load
          local.get 0
          i32.load offset=420
          i32.const 164
          i32.mul
          i32.add
          i32.const 16
          i32.add
          i32.load offset=52
          local.get 0
          call $j6737 ;; HighlightEffect.Fork
          local.get 2
          i32.load
          local.set 3
          local.get 0
          i32.load offset=420
          local.set 4
          local.get 3
          local.get 4
          i32.const 164
          i32.mul
          i32.add
          i32.const 16
          i32.add
          i32.load offset=52
          local.set 6
          local.get 3
          local.get 4
          i32.const 164
          i32.mul
          i32.add
          i32.const 16
          i32.add
          local.get 6
          i32.store offset=56
          local.get 2
          i32.load
          local.get 0
          i32.load offset=420
          i32.const 164
          i32.mul
          i32.add
          i32.const 16
          i32.add
          local.set 4
          local.get 4
          i32.load8_u offset=160
          if  ;; label = @4
            i32.const 0
            local.set 5
          else
            local.get 4
            i32.load offset=68
            i32.const 1
            i32.eq
            local.set 5
          end
          local.get 5
          i32.eqz
          if  ;; label = @4
            block  ;; label = @5
              block  ;; label = @6
                local.get 0
                f32.load offset=216
                f32.const 0x0p+0 (;=0;)
                f32.gt
                br_if 0 (;@6;)
                local.get 0
                f32.load offset=156
                f32.const 0x0p+0 (;=0;)
                f32.gt
                if  ;; label = @7
                  local.get 0
                  i32.load offset=164
                  i32.const 2
                  i32.ne
                  br_if 1 (;@6;)
                end
                local.get 0
                f32.load offset=116
                f32.const 0x0p+0 (;=0;)
                f32.gt
                i32.eqz
                br_if 1 (;@5;)
                local.get 0
                i32.load offset=140
                i32.const 2
                i32.eq
                br_if 1 (;@5;)
              end
              local.get 0
              i32.load offset=420
              local.set 4
              local.get 0
              i32.load offset=52
              i32.const -2
              i32.and
              i32.const 2
              i32.eq
              if  ;; label = @6
                i32.const 20504
                local.get 0
                local.get 4
                local.get 0
                call $redir_viii
                br 1 (;@5;)
              end
              local.get 0
              local.get 4
              local.get 0
              call $j39455 ;; HighlightEffect.AverageNormals
            end
            local.get 0
            local.get 0
            i32.load offset=420
            i32.const 1
            local.get 0
            call $j20236 ;; HighlightEffect.BakeTransform
          end
          local.get 0
          local.get 0
          i32.load offset=420
          i32.const 1
          i32.add
          i32.store offset=420
        end
        local.get 7
        i32.const 1
        i32.add
        local.set 7
        br 1 (;@1;)
      end
    end
    local.get 0
    i32.load8_u offset=36
    if  ;; label = @1
      local.get 0
      local.get 0
      call $j39447 ;; HighlightEffect.CombineMeshes
    end
    local.get 0
    local.get 0
    call $j9989 ;; HighlightEffect.UpdateMaterialPropertiesNow)
